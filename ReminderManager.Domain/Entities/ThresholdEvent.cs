using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReminderManager.Domain.Entities
{
    [Table("threshold_events")]
    public class ThresholdEvent
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("device_id")]
        public int? DeviceId { get; set; }

        [ForeignKey("DeviceId")]
        public ModbusDeviceConfig? Device { get; set; }



        [Column("column_name")]
        public string ColumnName { get; set; }


        [Column("actual_value")]
        public double ActualValue { get; set; }

        [Column("threshold_value")]
        public double ThresholdValue { get; set; }


        [Column("message")]
        public string Message { get; set; }

        [Column("triggered_at")]
        public DateTime TriggeredAt { get; set; }

    }
}
