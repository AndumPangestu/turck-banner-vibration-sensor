using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MQTTnet;
using ReminderManager.Domain.Entities;

namespace ModbusWorkerService
{
    // MQTT Publisher Service
    public interface IMqttPublisher
    {
        Task PublishAsync(MqttPayload payload, CancellationToken ct = default);
        Task ConnectAsync(CancellationToken ct = default);
        Task DisconnectAsync();
        bool IsConnected { get; }
    }

    public class MqttPublisher : IMqttPublisher, IAsyncDisposable
    {
        private readonly ILogger<MqttPublisher> _logger;
        private readonly MqttConfig _config;
        private IMqttClient _mqttClient;
        private readonly SemaphoreSlim _publishLock = new(1, 1);
        private bool _disposed;

        public bool IsConnected => _mqttClient?.IsConnected ?? false;

        public MqttPublisher(ILogger<MqttPublisher> logger, MqttConfig config)
        {
            _logger = logger;
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public async Task ConnectAsync(CancellationToken ct = default)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(MqttPublisher));

            try
            {
                if (IsConnected) return;

                var factory = new MqttClientFactory();
                _mqttClient = factory.CreateMqttClient();

                var optionsBuilder = new MqttClientOptionsBuilder()
                    .WithTcpServer(_config.Host, _config.Port)
                    .WithClientId(_config.ClientId)
                    .WithCleanSession();

                if (!string.IsNullOrEmpty(_config.Username))
                {
                    optionsBuilder.WithCredentials(_config.Username, _config.Password);
                }

                var options = optionsBuilder.Build();

                _logger.LogInformation("Connecting to MQTT broker at {Broker}:{Port}",
                    _config.Host, _config.Port);

                var result = await _mqttClient.ConnectAsync(options, ct);

                if (result.ResultCode == MqttClientConnectResultCode.Success)
                {
                    _logger.LogInformation("Successfully connected to MQTT broker");
                }
                else
                {
                    _logger.LogError("Failed to connect to MQTT broker: {ResultCode}", result.ResultCode);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error connecting to MQTT broker");
                throw;
            }
        }

        public async Task PublishAsync(MqttPayload payload, CancellationToken ct = default)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(MqttPublisher));

            await _publishLock.WaitAsync(ct);
            try
            {
                if (!IsConnected)
                {
                    _logger.LogWarning("MQTT client not connected, attempting to reconnect...");
                    await ConnectAsync(ct);
                }

                if (!IsConnected)
                {
                    _logger.LogError("Cannot publish: MQTT client not connected");
                    return;
                }

                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = false
                };

                var jsonPayload = JsonSerializer.Serialize(payload, jsonOptions);

                var message = new MqttApplicationMessageBuilder()
                    .WithTopic(_config.Topic)
                    .WithPayload(jsonPayload)
                    .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
                    .WithRetainFlag(false)
                    .Build();

                await _mqttClient.PublishAsync(message, ct);

                _logger.LogInformation("Published data for {Count} devices to MQTT topic '{Topic}'",
                    payload.Data.Count, _config.Topic);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error publishing to MQTT");
            }
            finally
            {
                _publishLock.Release();
            }
        }

        public async Task DisconnectAsync()
        {
            if (_mqttClient != null && IsConnected)
            {
                try
                {
                    await _mqttClient.DisconnectAsync();
                    _logger.LogInformation("Disconnected from MQTT broker");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error disconnecting from MQTT broker");
                }
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_disposed) return;

            await DisconnectAsync();
            _mqttClient?.Dispose();
            _publishLock?.Dispose();
            _disposed = true;
        }
    }
}
