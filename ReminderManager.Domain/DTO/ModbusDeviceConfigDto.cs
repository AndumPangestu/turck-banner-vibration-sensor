using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReminderManager.Domain.Entities;

namespace ReminderManager.Domain.DTO
{

    public class ModbusDeviceConfigFilterRequest
    {
        public string? Keyword { get; set; }

        public bool Paginate { get; set; } = false;

        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 10;
    }


    public static class ModbusDeviceConfigMapper
    {
        public static ModbusDeviceConfigResponse ToModbusDeviceConfigResponse(
            this ModbusDeviceConfig config)
        {
            if (config == null)
                return null!;

            return new ModbusDeviceConfigResponse
            {
                DeviceId = config.DeviceId,
                DeviceName = config.DeviceName,
                IpAddress = config.IpAddress,
                Port = config.Port,
                SlaveId = config.SlaveId,
                StartAddress = config.StartAddress,
                RegisterCount = config.RegisterCount,
                ReadIntervalMs = config.ReadIntervalMs,
                ConnectionTimeoutMs = config.ConnectionTimeoutMs,
                ReadTimeoutMs = config.ReadTimeoutMs,
                AutoReconnect = config.AutoReconnect,
                ReconnectDelayMs = config.ReconnectDelayMs,
                Enabled = config.Enabled,
                CreatedAt = config.CreatedAt,
                
            };
        }
    }


    public class ModbusDeviceConfigResponse
    {
        public int DeviceId { get; set; }
        public string DeviceName { get; set; }
        public string IpAddress { get; set; }
        public int Port { get; set; }
        public byte SlaveId { get; set; }
        public ushort StartAddress { get; set; }
        public ushort RegisterCount { get; set; }
        public int ReadIntervalMs { get; set; }
        public int ConnectionTimeoutMs { get; set; }
        public int ReadTimeoutMs { get; set; }
        public bool AutoReconnect { get; set; }
        public int ReconnectDelayMs { get; set; }
        public bool Enabled { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}
