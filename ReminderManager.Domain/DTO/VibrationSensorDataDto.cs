using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReminderManager.Domain.Entities;

namespace ReminderManager.Domain.DTO
{

    public class VibrationSensorDataFilterRequest
    {
        public int? DeviceId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public bool Paginate { get; set; } = false;

        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 10;
    }


    public static class VibrationSensorDataMapper
    {
        public static VibrationSensorDataResponse ToVibrationSensorDataResponse(this VibrationSensorData vibrationSensorData)
        {
            if (vibrationSensorData == null)
                return null!;

            return new VibrationSensorDataResponse
            {
                Id = vibrationSensorData.Id,
                DeviceId = vibrationSensorData.DeviceId,
                DeviceName = vibrationSensorData.Device?.DeviceName,
                VelocityX = vibrationSensorData.VelocityX,
                VelocityZ = vibrationSensorData.VelocityZ,
                AccelerationX = vibrationSensorData.AccelerationX,
                AccelerationZ = vibrationSensorData.AccelerationZ,
                Temperature = vibrationSensorData.Temperature,
                CreatedAt = vibrationSensorData.CreatedAt
            };
        }
    }

    public class VibrationSensorDataResponse
    {
        public int Id { get; set; }
        public int? DeviceId { get; set; }
        public string? DeviceName { get; set; }
        public double VelocityX { get; set; }
        public double VelocityZ { get; set; }
        public double AccelerationX { get; set; }
        public double AccelerationZ { get; set; }
        public double Temperature { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
