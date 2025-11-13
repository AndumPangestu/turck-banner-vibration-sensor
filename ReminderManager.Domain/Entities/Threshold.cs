using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReminderManager.Domain.Entities
{
    [Table("thresholds")]
    public class Threshold
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("device_id")]
        public int? DeviceId { get; set; }

        [ForeignKey("DeviceId")]
        public ModbusDeviceConfig? Device { get; set; }

        [Column("threshold_velocity_x")]
        public double ThresholdVelocityX { get; set; }

        [Column("message_threshold_velocity_x")]
        public string? MessageThresholdVelocityX { get; set; }


        [Column("threshold_velocity_z")]

        public double ThresholdVelocityZ { get; set; }

        [Column("message_threshold_velocity_z")]
        public string? MessageThresholdVelocityZ { get; set; }

        [Column("threshold_acceleration_x")]
        public double ThresholdAccelerationX { get; set; }

        [Column("message_threshold_acceleration_x")]
        public string? MessageThresholdAccelerationX { get; set; }

        [Column("threshold_acceleration_z")]
        public double ThresholdAccelerationZ { get; set; }

        [Column("message_threshold_acceleration_z")]
        public string? MessageThresholdAccelerationZ { get; set; }

        [Column("threshold_temperature")]
        public double ThresholdTemperature { get; set; }

        [Column("message_threshold_temperature")]
        public string? MessageThresholdTemperature { get; set; }


        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
