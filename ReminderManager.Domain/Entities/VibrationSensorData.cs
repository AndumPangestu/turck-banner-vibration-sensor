using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReminderManager.Domain.Entities
{
    [Table("vibration_sensor_data")]
    public class VibrationSensorData
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("device_id")]
        public int? DeviceId { get; set; }

        [ForeignKey("DeviceId")]
        public ModbusDeviceConfig? Device { get; set; }

        [Column("velocity_x")]
        public double VelocityX { get; set; }
        
        [Column("velocity_z")]
        public double VelocityZ { get; set; }

        [Column("acceleration_x")]
        public double AccelerationX { get; set; }

        [Column("acceleration_z")]
        public double AccelerationZ { get; set; }

        [Column("temperature")]
        public double Temperature { get; set; }
        

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
