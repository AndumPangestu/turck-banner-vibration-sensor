using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ReminderManager.Domain.Entities;
using ReminderManager.Infrastructure.Data;
using StackExchange.Redis;

namespace ModbusTcpWorkerService
{
    // Threshold Event Data Transfer Object
    public class ThresholdEventDto
    {
        public int DeviceId { get; set; }
        public string ColumnName { get; set; }
        public double ActualValue { get; set; }
        public double ThresholdValue { get; set; }
        public string Message { get; set; }
        public DateTime TriggeredAt { get; set; }
    }

    // Redis Cache Keys Helper
    public static class RedisCacheKeys
    {
        public static string ThresholdConfig(int deviceId) => $"threshold:config:{deviceId}";
        public static string LastTriggeredValue(int deviceId, string columnName) =>
            $"threshold:last:{deviceId}:{columnName}";
        public static string PendingEvents() => "threshold:pending:events";
        public static string EventCount(int deviceId) => $"threshold:count:{deviceId}";
    }

    // Interface for Threshold Service
    public interface IThresholdMonitoringService
    {
        Task CheckThresholdsAsync(ModbusDataSnapshot snapshot, CancellationToken ct);
        Task<List<ThresholdEvent>> GetRecentEventsAsync(int deviceId, int count = 10, CancellationToken ct = default);
        Task ClearDeviceCacheAsync(int deviceId, CancellationToken ct = default);
    }

    // Main Threshold Monitoring Service
    public class ThresholdMonitoringService : IThresholdMonitoringService
    {
        private readonly ILogger<ThresholdMonitoringService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IConnectionMultiplexer _redis;
        // Fully qualify the Redis IDatabase type to resolve ambiguity
        private readonly StackExchange.Redis.IDatabase _redisDb;
        private readonly TimeSpan _cacheExpiry = TimeSpan.FromHours(24);
        private readonly TimeSpan _lastValueExpiry = TimeSpan.FromMinutes(30);

        public ThresholdMonitoringService(
            ILogger<ThresholdMonitoringService> logger,
            IServiceScopeFactory scopeFactory,
            IConnectionMultiplexer redis)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _redis = redis;
            _redisDb = redis.GetDatabase();
        }

        public async Task CheckThresholdsAsync(ModbusDataSnapshot snapshot, CancellationToken ct)
        {
            if (!snapshot.IsConnected || !snapshot.IsSuccess || snapshot.HoldingRegisters.Length < 6)
            {
                return;
            }

            try
            {
                // Get threshold configuration from Redis or Database
                var threshold = await GetThresholdConfigAsync(snapshot.DeviceId, ct);
                if (threshold == null)
                {
                    _logger.LogDebug("[{DeviceId}] No threshold configuration found", snapshot.DeviceId);
                    return;
                }

                // Convert register values
                var temp = ConvertRegisterToDouble(snapshot.HoldingRegisters[0]);
                var accX = ConvertRegisterToDouble(snapshot.HoldingRegisters[1]);
                var accZ = ConvertRegisterToDouble(snapshot.HoldingRegisters[2]);
                var velX = ConvertRegisterToDouble(snapshot.HoldingRegisters[3]);
                var velZ = ConvertRegisterToDouble(snapshot.HoldingRegisters[4]);

                // Check each threshold
                var events = new List<ThresholdEventDto>();

                CheckAndAddEvent(events, snapshot.DeviceId, "VelocityX", velX,
                    threshold.ThresholdVelocityX, threshold.MessageThresholdVelocityX);

                CheckAndAddEvent(events, snapshot.DeviceId, "VelocityZ", velZ,
                    threshold.ThresholdVelocityZ, threshold.MessageThresholdVelocityZ);

                CheckAndAddEvent(events, snapshot.DeviceId, "AccelerationX", accX,
                    threshold.ThresholdAccelerationX, threshold.MessageThresholdAccelerationX);

                CheckAndAddEvent(events, snapshot.DeviceId, "AccelerationZ", accZ,
                    threshold.ThresholdAccelerationZ, threshold.MessageThresholdAccelerationZ);

                CheckAndAddEvent(events, snapshot.DeviceId, "Temperature", temp,
                    threshold.ThresholdTemperature, threshold.MessageThresholdTemperature);

                // Process triggered events
                if (events.Any())
                {
                    await ProcessTriggeredEventsAsync(events, ct);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[{DeviceId}] Error checking thresholds", snapshot.DeviceId);
            }
        }

        private void CheckAndAddEvent(List<ThresholdEventDto> events, int deviceId,
            string columnName, double actualValue, double thresholdValue, string message)
        {
            // Skip if threshold is 0 (disabled) or message is empty
            if (thresholdValue <= 0 || string.IsNullOrEmpty(message))
            {
                return;
            }

            // Check if threshold is exceeded
            if (Math.Abs(actualValue) > Math.Abs(thresholdValue))
            {
                events.Add(new ThresholdEventDto
                {
                    DeviceId = deviceId,
                    ColumnName = columnName,
                    ActualValue = actualValue,
                    ThresholdValue = thresholdValue,
                    Message = message,
                    TriggeredAt = DateTime.UtcNow
                });
            }
        }

        private async Task ProcessTriggeredEventsAsync(List<ThresholdEventDto> events, CancellationToken ct)
        {
            var eventsToSave = new List<ThresholdEventDto>();

            foreach (var evt in events)
            {
                try
                {
                    // Check if value changed from last triggered value
                    var lastValueKey = RedisCacheKeys.LastTriggeredValue(evt.DeviceId, evt.ColumnName);
                    var lastValueStr = await _redisDb.StringGetAsync(lastValueKey);

                    bool shouldSave = false;

                    if (lastValueStr.IsNullOrEmpty)
                    {
                        // First time triggering
                        shouldSave = true;
                    }
                    else
                    {
                        var lastValue = double.Parse(lastValueStr);
                        var valueDifference = Math.Abs(evt.ActualValue - lastValue);
                        var changePercentage = Math.Abs(valueDifference / lastValue) * 100;

                        // Save if value changed by more than 5% or absolute difference > 1
                        shouldSave = changePercentage > 5 || valueDifference > 1;
                    }

                    if (shouldSave)
                    {
                        // Update last value in Redis
                        await _redisDb.StringSetAsync(
                            lastValueKey,
                            evt.ActualValue.ToString(),
                            _lastValueExpiry);

                        eventsToSave.Add(evt);

                        _logger.LogWarning(
                            "[{DeviceId}] Threshold exceeded - {Column}: {Actual:F2} > {Threshold:F2} - {Message}",
                            evt.DeviceId, evt.ColumnName, evt.ActualValue, evt.ThresholdValue, evt.Message);
                    }
                    else
                    {
                        _logger.LogDebug(
                            "[{DeviceId}] Threshold still exceeded but value unchanged - {Column}: {Actual:F2}",
                            evt.DeviceId, evt.ColumnName, evt.ActualValue);
                    }

                    // Store in Redis pending queue for batch processing
                    if (shouldSave)
                    {
                        var eventJson = JsonSerializer.Serialize(evt);
                        await _redisDb.ListRightPushAsync(RedisCacheKeys.PendingEvents(), eventJson);

                        // Increment event counter
                        await _redisDb.StringIncrementAsync(RedisCacheKeys.EventCount(evt.DeviceId));
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "[{DeviceId}] Error processing threshold event for {Column}",
                        evt.DeviceId, evt.ColumnName);
                }
            }

            // Batch save to database if we have events
            if (eventsToSave.Any())
            {
                await SaveEventsToDatabaseAsync(eventsToSave, ct);
            }
        }

        private async Task SaveEventsToDatabaseAsync(List<ThresholdEventDto> events, CancellationToken ct)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var entities = events.Select(e => new ThresholdEvent
                {
                    DeviceId = e.DeviceId,
                    ColumnName = e.ColumnName,
                    ActualValue = e.ActualValue,
                    ThresholdValue = e.ThresholdValue,
                    Message = e.Message,
                    TriggeredAt = e.TriggeredAt
                }).ToList();

                await db.ThresholdEvent.AddRangeAsync(entities, ct);
                await db.SaveChangesAsync(ct);

                _logger.LogInformation("Saved {Count} threshold events to database", entities.Count);

                // Remove from Redis pending queue
                var pendingKey = RedisCacheKeys.PendingEvents();
                await _redisDb.KeyDeleteAsync(pendingKey);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save threshold events to database");
                // Events remain in Redis queue for retry
            }
        }

        private async Task<Threshold> GetThresholdConfigAsync(int deviceId, CancellationToken ct)
        {
            var cacheKey = RedisCacheKeys.ThresholdConfig(deviceId);

            try
            {
                // Try to get from Redis cache
                var cachedValue = await _redisDb.StringGetAsync(cacheKey);

                if (!cachedValue.IsNullOrEmpty)
                {
                    return JsonSerializer.Deserialize<Threshold>(cachedValue);
                }

                // If not in cache, get from database
                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var threshold = await db.Threshold
                    .FirstOrDefaultAsync(t => t.DeviceId == deviceId, ct);

                if (threshold != null)
                {
                    // Cache in Redis
                    var json = JsonSerializer.Serialize(threshold);
                    await _redisDb.StringSetAsync(cacheKey, json, _cacheExpiry);

                    _logger.LogDebug("[{DeviceId}] Threshold config cached in Redis", deviceId);
                }

                return threshold;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[{DeviceId}] Error getting threshold configuration", deviceId);
                return null;
            }
        }

        public async Task<List<ThresholdEvent>> GetRecentEventsAsync(
            int deviceId, int count = 10, CancellationToken ct = default)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                return await db.ThresholdEvent
                    .Where(e => e.DeviceId == deviceId)
                    .OrderByDescending(e => e.TriggeredAt)
                    .Take(count)
                    .ToListAsync(ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[{DeviceId}] Error getting recent events", deviceId);
                return new List<ThresholdEvent>();
            }
        }

        public async Task ClearDeviceCacheAsync(int deviceId, CancellationToken ct = default)
        {
            try
            {
                var tasks = new List<Task>
                {
                    _redisDb.KeyDeleteAsync(RedisCacheKeys.ThresholdConfig(deviceId)),
                    _redisDb.KeyDeleteAsync(RedisCacheKeys.EventCount(deviceId))
                };

                // Clear all last triggered values for this device
                var server = _redis.GetServer(_redis.GetEndPoints().First());
                var keys = server.Keys(pattern: $"threshold:last:{deviceId}:*");

                foreach (var key in keys)
                {
                    tasks.Add(_redisDb.KeyDeleteAsync(key));
                }

                await Task.WhenAll(tasks);

                _logger.LogInformation("[{DeviceId}] Redis cache cleared", deviceId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[{DeviceId}] Error clearing cache", deviceId);
            }
        }

        private double ConvertRegisterToDouble(ushort register)
        {
            short signedValue = unchecked((short)register);
            return signedValue;
        }
    }

    // Background service to persist pending events periodically
    public class ThresholdPersistenceService : BackgroundService
    {
        private readonly ILogger<ThresholdPersistenceService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IConnectionMultiplexer _redis;
        private readonly TimeSpan _persistInterval = TimeSpan.FromMinutes(1);

        public ThresholdPersistenceService(
            ILogger<ThresholdPersistenceService> logger,
            IServiceScopeFactory scopeFactory,
            IConnectionMultiplexer redis)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _redis = redis;
        }

        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            _logger.LogInformation("Threshold Persistence Service started");

            while (!ct.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(_persistInterval, ct);
                    await PersistPendingEventsAsync(ct);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in persistence service");
                }
            }

            _logger.LogInformation("Threshold Persistence Service stopped");
        }

        private async Task PersistPendingEventsAsync(CancellationToken ct)
        {
            var db = _redis.GetDatabase();
            var pendingKey = RedisCacheKeys.PendingEvents();

            try
            {
                var length = await db.ListLengthAsync(pendingKey);
                if (length == 0) return;

                _logger.LogInformation("Persisting {Count} pending threshold events", length);

                var events = new List<ThresholdEventDto>();

                // Get all pending events
                var values = await db.ListRangeAsync(pendingKey);

                foreach (var value in values)
                {
                    if (!value.IsNullOrEmpty)
                    {
                        var evt = JsonSerializer.Deserialize<ThresholdEventDto>(value);
                        events.Add(evt);
                    }
                }

                if (events.Any())
                {
                    using var scope = _scopeFactory.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    var entities = events.Select(e => new ThresholdEvent
                    {
                        DeviceId = e.DeviceId,
                        ColumnName = e.ColumnName,
                        ActualValue = e.ActualValue,
                        ThresholdValue = e.ThresholdValue,
                        Message = e.Message,
                        TriggeredAt = e.TriggeredAt
                    }).ToList();

                    await dbContext.ThresholdEvent.AddRangeAsync(entities, ct);
                    await dbContext.SaveChangesAsync(ct);

                    // Clear the queue after successful save
                    await db.KeyDeleteAsync(pendingKey);

                    _logger.LogInformation("Successfully persisted {Count} threshold events", entities.Count);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to persist pending events");
                // Events remain in Redis for retry
            }
        }

        public override async Task StopAsync(CancellationToken ct)
        {
            // Final persistence before shutdown
            await PersistPendingEventsAsync(ct);
            await base.StopAsync(ct);
        }
    }
}