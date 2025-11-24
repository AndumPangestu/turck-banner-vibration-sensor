using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModbusTcpWorkerService;
using ModbusWorkerService;
using MQTTnet;
using MQTTnet.Diagnostics.Logger;
using MQTTnet.Exceptions;
using MQTTnet.Internal;
using MQTTnet.Protocol;
using NModbus;
using NModbus.Serial;
using ReminderManager.Application.Interfaces;
using ReminderManager.Domain.Entities;
using ReminderManager.Infrastructure.Data;
using StackExchange.Redis;

namespace ModbusRtuWorkerService
{
    // Data Handler Interface
    public interface IModbusDataHandler
    {
        Task HandleDataAsync(ModbusDataSnapshot snapshot, CancellationToken ct);
        MqttDeviceData GetLatestDeviceData(int deviceId);
    }

    // Handler Implementation with MQTT Data Tracking and Threshold Monitoring
    public class ModbusDataHandler : IModbusDataHandler
    {
        private readonly ILogger<ModbusDataHandler> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IThresholdMonitoringService _thresholdService;
        private readonly ConcurrentDictionary<int, MqttDeviceData> _latestData = new();

        public ModbusDataHandler(
            ILogger<ModbusDataHandler> logger,
            IServiceScopeFactory scopeFactory,
            IThresholdMonitoringService thresholdService)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _thresholdService = thresholdService;
        }

        public async Task HandleDataAsync(ModbusDataSnapshot snapshot, CancellationToken ct)
        {
            var devicePrefix = $"[{snapshot.DeviceId}]";

            

            // Create MQTT data entry
            var mqttData = new MqttDeviceData
            {
                DeviceName = snapshot.DeviceName,
                Timestamp = snapshot.Timestamp,
                Status = snapshot.IsConnected ? "Connected" : "Disconnected"
            };

            if (!snapshot.IsConnected)
            {
                _logger.LogWarning("{Device} Not connected at {Timestamp}",
                    devicePrefix, snapshot.Timestamp);
                _latestData[snapshot.DeviceId] = mqttData;
                return;
            }

            if (!snapshot.IsSuccess)
            {
                _logger.LogError("{Device} Read failed: {Error}",
                    devicePrefix, snapshot.ErrorMessage);
                mqttData.Status = "Error";
                _latestData[snapshot.DeviceId] = mqttData;
                return;
            }

            _logger.LogInformation("{Device} Read {Count} registers - {DeviceName}",
                devicePrefix, snapshot.HoldingRegisters.Length, snapshot.DeviceName);

            if (snapshot.HoldingRegisters.Length >= 7)
            {

                // Convert ushort to double
                mqttData.VelX = ConvertRegisterToDouble(snapshot.HoldingRegisters[0]);
                mqttData.AccX = ConvertRegisterToDouble(snapshot.HoldingRegisters[1]);
                mqttData.VelY = ConvertRegisterToDouble(snapshot.HoldingRegisters[2]);
                mqttData.AccY = ConvertRegisterToDouble(snapshot.HoldingRegisters[3]);
                mqttData.VelZ = ConvertRegisterToDouble(snapshot.HoldingRegisters[4]);
                mqttData.AccZ = ConvertRegisterToDouble(snapshot.HoldingRegisters[5]);
                mqttData.Temp = ConvertRegisterToDouble(snapshot.HoldingRegisters[6]);

                var threshold = await _thresholdService.GetThresholdConfigAsync(snapshot.DeviceId, ct);
                if (threshold != null)
                {
                    mqttData.ThresholdVelX = threshold.ThresholdVelocityX;
                    mqttData.ThresholdAccX = threshold.ThresholdAccelerationX;
                    mqttData.ThresholdVelY = threshold.ThresholdVelocityY;
                    mqttData.ThresholdAccY = threshold.ThresholdAccelerationY;
                    mqttData.ThresholdVelZ = threshold.ThresholdVelocityZ;
                    mqttData.ThresholdAccZ = threshold.ThresholdAccelerationZ;
                    mqttData.ThresholdTemp = threshold.ThresholdTemperature;
                }

                _latestData[snapshot.DeviceId] = mqttData;

                // Save sensor data to database
                await SaveSensorDataAsync(snapshot, ct);

                // Check thresholds (async, no need to wait)
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await _thresholdService.CheckThresholdsAsync(snapshot, ct);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "{Device} Threshold check failed", devicePrefix);
                    }
                }, ct);
            }
        }

        private double ConvertRegisterToDouble(ushort register)
        {
            short signedValue = unchecked((short)register);
            return signedValue;
        }

        public MqttDeviceData GetLatestDeviceData(int deviceId)
        {
            return _latestData.TryGetValue(deviceId, out var data) ? data : null;
        }

        private async Task SaveSensorDataAsync(ModbusDataSnapshot snapshot, CancellationToken ct)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var sensorData = new VibrationSensorData
                {
                    DeviceId = snapshot.DeviceId,
                    VelocityX = snapshot.HoldingRegisters[0],
                    AccelerationX = snapshot.HoldingRegisters[1],
                    VelocityY = snapshot.HoldingRegisters[2],
                    AccelerationY = snapshot.HoldingRegisters[3],
                    VelocityZ = snapshot.HoldingRegisters[4],
                    AccelerationZ = snapshot.HoldingRegisters[5],
                    Temperature = snapshot.HoldingRegisters[6],
                    CreatedAt = DateTime.UtcNow
                };

                await db.VibrationSensorData.AddAsync(sensorData, ct);
                await db.SaveChangesAsync(ct);

                _logger.LogDebug("[{DeviceId}] Data saved - Temp={Temp}, AccX={AccX}, AccZ={AccZ}",
                    snapshot.DeviceId, sensorData.Temperature,
                    sensorData.AccelerationX, sensorData.AccelerationZ);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[{DeviceId}] Failed to save sensor data", snapshot.DeviceId);
            }
        }
    }

    // Modbus RTU Device Connection Manager
    public class ModbusDeviceConnection : IAsyncDisposable
    {
        private readonly ILogger<ModbusDeviceConnection> _logger;
        private readonly ModbusDeviceConfig _config;
        private SerialPort _serialPort;
        private IModbusMaster _modbusMaster;
        private readonly SemaphoreSlim _connectionLock = new(1, 1);
        private bool _isConnected;
        private bool _disposed;

        public int DeviceId => _config.DeviceId;
        public string DeviceName => _config.DeviceName;
        public bool IsConnected => _isConnected && _serialPort?.IsOpen == true;

        public ModbusDeviceConnection(
            ILogger<ModbusDeviceConnection> logger,
            ModbusDeviceConfig config)
        {
            _logger = logger;
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public async Task<bool> ConnectAsync(CancellationToken ct)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(ModbusDeviceConnection));

            await _connectionLock.WaitAsync(ct);
            try
            {
                if (IsConnected) return true;

                await DisconnectInternalAsync();

                // Validate port exists
                var availablePorts = SerialPort.GetPortNames();
                if (!availablePorts.Contains(_config.PortName))
                {
                    _logger.LogError("[{DeviceId}] Port {PortName} not found. Available ports: {Ports}",
                        _config.DeviceId, _config.PortName, string.Join(", ", availablePorts));
                    return false;
                }

                _logger.LogInformation("[{DeviceId}] Connecting to {DeviceName} on {PortName} at {BaudRate} baud",
                    _config.DeviceId, _config.DeviceName, _config.PortName, _config.BaudRate);

                try
                {
                    _serialPort = new SerialPort
                    {
                        PortName = _config.PortName,
                        BaudRate = _config.BaudRate,
                        DataBits = _config.DataBits,
                        Parity = _config.Parity,
                        StopBits = _config.StopBits,
                        ReadTimeout = _config.ReadTimeoutMs,
                        WriteTimeout = _config.ReadTimeoutMs,
                        Handshake = Handshake.None,
                        RtsEnable = false,
                        DtrEnable = false
                    };

                    _serialPort.Open();

                    // Wait a bit for port to stabilize
                    await Task.Delay(100, ct);

                    var factory = new ModbusFactory();
                    _modbusMaster = factory.CreateRtuMaster(_serialPort);
                    _modbusMaster.Transport.ReadTimeout = _config.ReadTimeoutMs;
                    _modbusMaster.Transport.WriteTimeout = _config.ReadTimeoutMs;
                    _modbusMaster.Transport.Retries = 3;
                    _modbusMaster.Transport.WaitToRetryMilliseconds = 250;

                    _isConnected = true;
                    _logger.LogInformation("[{DeviceId}] Connected to {DeviceName}",
                        _config.DeviceId, _config.DeviceName);

                    return true;
                }
                catch (UnauthorizedAccessException ex)
                {
                    _logger.LogError(ex, "[{DeviceId}] Port {PortName} is already in use by another application",
                        _config.DeviceId, _config.PortName);
                    return false;
                }
                catch (ArgumentException ex)
                {
                    _logger.LogError(ex, "[{DeviceId}] Invalid port configuration: {Message}",
                        _config.DeviceId, ex.Message);
                    return false;
                }
                catch (IOException ex)
                {
                    _logger.LogError(ex, "[{DeviceId}] Device not functioning or disconnected: {Message}",
                        _config.DeviceId, ex.Message);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[{DeviceId}] Connection failed", _config.DeviceId);
                await DisconnectInternalAsync();
                return false;
            }
            finally
            {
                _connectionLock.Release();
            }
        }

        public async Task<ModbusDataSnapshot> ReadHoldingRegistersAsync(CancellationToken ct)
        {
            var snapshot = new ModbusDataSnapshot
            {
                DeviceId = _config.DeviceId,
                DeviceName = _config.DeviceName,
                Timestamp = DateTime.UtcNow,
                IsConnected = IsConnected
            };

            if (!IsConnected)
            {
                snapshot.IsSuccess = false;
                snapshot.ErrorMessage = "Not connected";
                return snapshot;
            }

            try
            {
                // Add small delay before reading (RTU requires timing)
                await Task.Delay(50, ct);

                snapshot.HoldingRegisters = await _modbusMaster.ReadHoldingRegistersAsync(
                    _config.SlaveId, _config.StartAddress, _config.RegisterCount);
                snapshot.IsSuccess = true;
            }
            catch (Exception ex)
            {
                snapshot.IsSuccess = false;
                snapshot.ErrorMessage = ex.Message;
                _isConnected = false;
                _logger.LogWarning(ex, "[{DeviceId}] Read failed", _config.DeviceId);
            }

            return snapshot;
        }

        private async Task DisconnectInternalAsync()
        {
            _modbusMaster?.Dispose();
            _modbusMaster = null;

            if (_serialPort?.IsOpen == true)
            {
                _serialPort?.Close();
            }
            _serialPort?.Dispose();
            _serialPort = null;

            _isConnected = false;
            await Task.CompletedTask;
        }

        public async Task DisconnectAsync()
        {
            await _connectionLock.WaitAsync();
            try
            {
                await DisconnectInternalAsync();
                _logger.LogInformation("[{DeviceId}] Disconnected", _config.DeviceId);
            }
            finally
            {
                _connectionLock.Release();
            }
        }

        public int GetReadInterval() => _config.ReadIntervalMs;
        public bool ShouldAutoReconnect() => _config.AutoReconnect;
        public int GetReconnectDelay() => _config.ReconnectDelayMs;

        public async ValueTask DisposeAsync()
        {
            if (_disposed) return;

            await DisconnectAsync();
            _connectionLock?.Dispose();
            _disposed = true;
        }
    }

    // Single Device Worker
    public class ModbusDeviceWorker : BackgroundService
    {
        private readonly ILogger<ModbusDeviceWorker> _logger;
        private readonly ModbusDeviceConnection _connection;
        private readonly IModbusDataHandler _dataHandler;

        public int DeviceId => _connection.DeviceId;

        public ModbusDeviceWorker(
            ILogger<ModbusDeviceWorker> logger,
            ModbusDeviceConnection connection,
            IModbusDataHandler dataHandler)
        {
            _logger = logger;
            _connection = connection;
            _dataHandler = dataHandler;
        }

        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            _logger.LogInformation("[{DeviceId}] Worker started for {DeviceName}",
                _connection.DeviceId, _connection.DeviceName);

            while (!ct.IsCancellationRequested)
            {
                try
                {
                    await EnsureConnectionAsync(ct);

                    if (_connection.IsConnected)
                    {
                        var snapshot = await _connection.ReadHoldingRegistersAsync(ct);
                        await _dataHandler.HandleDataAsync(snapshot, ct);
                    }

                    await Task.Delay(_connection.GetReadInterval(), ct);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "[{DeviceId}] Error in polling cycle", _connection.DeviceId);
                    await Task.Delay(1000, ct);
                }
            }

            _logger.LogInformation("[{DeviceId}] Worker stopped", _connection.DeviceId);
        }

        private async Task EnsureConnectionAsync(CancellationToken ct)
        {
            if (_connection.IsConnected) return;

            var connected = await _connection.ConnectAsync(ct);

            if (!connected && _connection.ShouldAutoReconnect())
            {
                await Task.Delay(_connection.GetReconnectDelay(), ct);
            }
        }

        public override async Task StopAsync(CancellationToken ct)
        {
            await _connection.DisconnectAsync();
            await base.StopAsync(ct);
        }

        public override void Dispose()
        {
            _connection?.DisposeAsync().AsTask().GetAwaiter().GetResult();
            base.Dispose();
        }
    }

    // Multi-Device Manager with MQTT Publishing
    public class ModbusMultiDeviceManager : IHostedService
    {
        private readonly ILogger<ModbusMultiDeviceManager> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IMqttPublisher _mqttPublisher;
        private readonly IModbusDataHandler _dataHandler;
        private readonly IThresholdMonitoringService _thresholdService;
        private readonly ConcurrentDictionary<int, WorkerContext> _workers = new();
        private readonly SemaphoreSlim _managementLock = new(1, 1);
        private Timer _mqttPublishTimer;
        private readonly int _mqttPublishIntervalMs = 1000;

        private class WorkerContext
        {
            public ModbusDeviceWorker Worker { get; set; }
            public CancellationTokenSource Cts { get; set; }
        }
        public ModbusMultiDeviceManager(
            ILogger<ModbusMultiDeviceManager> logger,
            IServiceProvider serviceProvider,
            IMqttPublisher mqttPublisher,
            IModbusDataHandler dataHandler,
            IThresholdMonitoringService thresholdService)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _mqttPublisher = mqttPublisher;
            _dataHandler = dataHandler;
            _thresholdService = thresholdService;
        }

        public async Task StartAsync(CancellationToken ct)
        {
            _logger.LogInformation("Starting Modbus RTU Multi-Device Manager");

            // Connect to MQTT broker
            try
            {
                await _mqttPublisher.ConnectAsync(ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to connect to MQTT broker");
            }

            using var scope = _serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var enabledDevices = await db.ModbusDeviceConfig
                .Where(d => d.Enabled)
                .Include(d => d.Threshold)
                .ToListAsync(ct);

            _logger.LogInformation("Found {Count} enabled devices", enabledDevices.Count);

            foreach (var deviceConfig in enabledDevices)
            {
                await AddDeviceAsync(deviceConfig, ct);
                await _thresholdService.SetThresholdConfigAsync(deviceConfig.DeviceId, deviceConfig.Threshold, ct);
            }

            // Start MQTT publish timer
            _mqttPublishTimer = new Timer(
                PublishToMqttCallback,
                null,
                TimeSpan.FromMilliseconds(_mqttPublishIntervalMs),
                TimeSpan.FromMilliseconds(_mqttPublishIntervalMs));

            _logger.LogInformation("All enabled devices started, MQTT publishing enabled");
        }

        private async void PublishToMqttCallback(object state)
        {
            try
            {
                await PublishAllDeviceDataToMqttAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in MQTT publish callback");
            }
        }

        private async Task PublishAllDeviceDataToMqttAsync()
        {
            try
            {
                var payload = new MqttPayload
                {
                    SentAt = DateTime.UtcNow
                };

                // Collect data from all active devices
                foreach (var deviceId in _workers.Keys)
                {
                    var deviceData = _dataHandler.GetLatestDeviceData(deviceId);
                    if (deviceData != null)
                    {
                        payload.Data.Add(deviceData);
                    }
                }

                if (payload.Data.Any())
                {
                    await _mqttPublisher.PublishAsync(payload);
                }
                else
                {
                    _logger.LogDebug("No device data available to publish");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error publishing data to MQTT");
            }
        }

        public async Task<bool> AddDeviceAsync(ModbusDeviceConfig config, CancellationToken ct = default)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));

            await _managementLock.WaitAsync(ct);
            try
            {
                if (_workers.ContainsKey(config.DeviceId))
                {
                    _logger.LogWarning("[{DeviceId}] Device already exists", config.DeviceId);
                    return false;
                }

                var connectionLogger = _serviceProvider.GetRequiredService<ILogger<ModbusDeviceConnection>>();
                var connection = new ModbusDeviceConnection(connectionLogger, config);

                var workerLogger = _serviceProvider.GetRequiredService<ILogger<ModbusDeviceWorker>>();
                var worker = new ModbusDeviceWorker(workerLogger, connection, _dataHandler);

                var cts = new CancellationTokenSource();
                await worker.StartAsync(cts.Token);

                _workers[config.DeviceId] = new WorkerContext
                {
                    Worker = worker,
                    Cts = cts
                };

                _logger.LogInformation("[{DeviceId}] Device {DeviceName} added successfully",
                    config.DeviceId, config.DeviceName);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[{DeviceId}] Failed to add device", config.DeviceId);
                return false;
            }
            finally
            {
                _managementLock.Release();
            }
        }

        public async Task<bool> RemoveDeviceAsync(int deviceId, CancellationToken ct = default)
        {
            await _managementLock.WaitAsync(ct);
            try
            {
                if (!_workers.TryRemove(deviceId, out var context))
                {
                    _logger.LogWarning("[{DeviceId}] Device not found", deviceId);
                    return false;
                }

                context.Cts.Cancel();
                await context.Worker.StopAsync(ct);
                context.Worker.Dispose();
                context.Cts.Dispose();

                _logger.LogInformation("[{DeviceId}] Device removed successfully", deviceId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[{DeviceId}] Failed to remove device", deviceId);
                return false;
            }
            finally
            {
                _managementLock.Release();
            }
        }

        public async Task<bool> StopDeviceAsync(int deviceId, CancellationToken ct = default)
        {
            if (!_workers.TryGetValue(deviceId, out var context))
            {
                _logger.LogWarning("[{DeviceId}] Device not found", deviceId);
                return false;
            }

            try
            {
                context.Cts.Cancel();
                await context.Worker.StopAsync(ct);
                _logger.LogInformation("[{DeviceId}] Device stopped", deviceId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[{DeviceId}] Failed to stop device", deviceId);
                return false;
            }
        }

        public async Task<bool> UpdateDeviceStatusAsync(int deviceId, bool enabled, CancellationToken ct = default)
        {
            using var scope = _serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var device = await db.ModbusDeviceConfig.FindAsync(new object[] { deviceId }, ct);
            if (device == null)
            {
                _logger.LogWarning("[{DeviceId}] Device not found in database", deviceId);
                return false;
            }

            device.Enabled = enabled;
            await db.SaveChangesAsync(ct);

            if (enabled)
            {
                if (!_workers.ContainsKey(deviceId))
                {
                    return await AddDeviceAsync(device, ct);
                }
            }
            else
            {
                if (_workers.ContainsKey(deviceId))
                {
                    return await RemoveDeviceAsync(deviceId, ct);
                }
            }

            return true;
        }

        public IReadOnlyDictionary<int, string> GetRunningDevices()
        {
            return _workers.ToDictionary(
                kvp => kvp.Key,
                kvp => $"Device {kvp.Key}"
            );
        }

        public async Task StopAsync(CancellationToken ct)
        {
            _logger.LogInformation("Stopping all device workers");

            // Stop MQTT publish timer
            _mqttPublishTimer?.Change(Timeout.Infinite, Timeout.Infinite);
            _mqttPublishTimer?.Dispose();

            var stopTasks = _workers.Select(async kvp =>
            {
                try
                {
                    kvp.Value.Cts.Cancel();
                    await kvp.Value.Worker.StopAsync(ct);
                    kvp.Value.Worker.Dispose();
                    kvp.Value.Cts.Dispose();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "[{DeviceId}] Error stopping worker", kvp.Key);
                }
            });

            await Task.WhenAll(stopTasks);
            _workers.Clear();

            // Disconnect MQTT
            await _mqttPublisher.DisconnectAsync();

            _logger.LogInformation("All device workers stopped");
        }
    }

    // Extension Methods for Service Registration
    public static class ModbusServiceExtensions
    {
        public static IServiceCollection AddModbusServices(
            this IServiceCollection services,
            string connectionString,
            string redisConnectionString,
            MqttConfig mqttConfig = null)
        {
            // Database Context
            services.AddDbContext<AppDbContext>(opts =>
            {
                opts.UseMySql(
                    connectionString,
                    ServerVersion.AutoDetect(connectionString),
                    mysqlopts => mysqlopts.MigrationsAssembly("ReminderManager.Infrastructure")
                );
            });

            // Redis Connection
            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var configuration = ConfigurationOptions.Parse(redisConnectionString);
                configuration.AbortOnConnectFail = false;
                configuration.ConnectRetry = 3;
                configuration.ConnectTimeout = 5000;
                configuration.SyncTimeout = 5000;

                return ConnectionMultiplexer.Connect(configuration);
            });

            // MQTT Configuration
            var config = mqttConfig ?? new MqttConfig();
            services.AddSingleton(config);

            // Modbus Services
            services.AddSingleton<IModbusDataHandler, ModbusDataHandler>();
            services.AddSingleton<IMqttPublisher, MqttPublisher>();
            services.AddSingleton<IThresholdMonitoringService, ThresholdMonitoringService>();

            // Background Services
            services.AddSingleton<ModbusMultiDeviceManager>();
            services.AddHostedService(provider => provider.GetRequiredService<ModbusMultiDeviceManager>());
            services.AddHostedService<ThresholdPersistenceService>();

            return services;
        }
    }
}