using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReminderManager.Domain.Entities
{
    [Table("modbus_device_configs")]
    public class ModbusDeviceConfig
    {

        [Key]
        [Column("device_id")]
        public int DeviceId { get; set; }

        [Column("device_name")]
        public string DeviceName { get; set; }

        [Column("ip_address")]
        public string IpAddress { get; set; }

        [Column("port")]
        public int Port { get; set; } = 502;

        [Column("slave_id")]
        public byte SlaveId { get; set; } = 1;

        [Column("start_address")]
        public ushort StartAddress { get; set; } = 0;

        [Column("register_count")]
        public ushort RegisterCount { get; set; } = 16;

        [Column("read_interval_ms")]
        public int ReadIntervalMs { get; set; } = 500;

        [Column("connection_timeout_ms")]
        public int ConnectionTimeoutMs { get; set; } = 5000;

        [Column("read_timeout_ms")]
        public int ReadTimeoutMs { get; set; } = 1000;

        [Column("auto_reconnect")]
        public bool AutoReconnect { get; set; } = true;

        [Column("reconnect_delay_ms")]
        public int ReconnectDelayMs { get; set; } = 3000;

        [Column("enabled")]
        public bool Enabled { get; set; } = true;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<VibrationSensorData> VibrationSensorDataList { get; set; }

        public ICollection<ThresholdEvent> ThresholdEvents { get; set; }

        public Threshold? Threshold { get; set; }
    }

    // Application Configuration
    public class ModbusAppConfig
    {
        public List<ModbusDeviceConfig> Devices { get; set; } = new();
    }

    public class ModbusDataSnapshot
    {
        public int DeviceId { get; set; }
        public string DeviceName { get; set; }
        public DateTime Timestamp { get; set; }
        public ushort[] HoldingRegisters { get; set; }
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public bool IsConnected { get; set; }
    }
}
