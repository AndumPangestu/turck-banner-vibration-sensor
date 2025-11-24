using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReminderManager.Domain.Entities;

namespace ReminderManager.Domain.DTO
{
    public class ThresholdFilterRequest
    {
        public string? Keyword { get; set; }
        public bool Paginate { get; set; } = false;

        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 10;
    }

    public class ThresholdRequest
    {
        public int DeviceId { get; set; }
        public double ThresholdVelocityX { get; set; }
        public string? MessageThresholdVelocityX { get; set; }
        public double ThresholdVelocityZ { get; set; }
        public string? MessageThresholdVelocityZ { get; set; }

        public double ThresholdVelocityY { get; set; }
        public string? MessageThresholdVelocityY { get; set; }

        public double ThresholdAccelerationX { get; set; }
        public string? MessageThresholdAccelerationX { get; set; }
        public double ThresholdAccelerationZ { get; set; }
        public string? MessageThresholdAccelerationZ { get; set; }

        public double ThresholdAccelerationY { get; set; }
        public string? MessageThresholdAccelerationY { get; set; }

        public double ThresholdTemperature { get; set; }
        public string? MessageThresholdTemperature { get; set; }    
    }


    public static class ThresholdMapper
    {
        public static ThresholdResponse ToThresholdResponse(this Threshold vibrationSensorData)
        {
            if (vibrationSensorData == null)
                return null!;

            return new ThresholdResponse
            {
                Id = vibrationSensorData.Id,
                DeviceId = vibrationSensorData.DeviceId,
                DeviceName = vibrationSensorData.Device?.DeviceName,
                ThresholdVelocityX = vibrationSensorData.ThresholdVelocityX,
                MessageThresholdVelocityX = vibrationSensorData.MessageThresholdVelocityX,
                ThresholdVelocityZ = vibrationSensorData.ThresholdVelocityZ,
                MessageThresholdVelocityZ = vibrationSensorData.MessageThresholdVelocityZ,
                ThresholdVelocityY = vibrationSensorData.ThresholdVelocityY,
                MessageThresholdVelocityY = vibrationSensorData.MessageThresholdVelocityY,
                ThresholdAccelerationX = vibrationSensorData.ThresholdAccelerationX,
                MessageThresholdAccelerationX = vibrationSensorData.MessageThresholdAccelerationX,
                ThresholdAccelerationZ = vibrationSensorData.ThresholdAccelerationZ,
                MessageThresholdAccelerationZ = vibrationSensorData.MessageThresholdAccelerationZ,
                ThresholdAccelerationY = vibrationSensorData.ThresholdAccelerationY,
                MessageThresholdAccelerationY = vibrationSensorData.MessageThresholdAccelerationY,
                ThresholdTemperature = vibrationSensorData.ThresholdTemperature,
                MessageThresholdTemperature = vibrationSensorData.MessageThresholdTemperature,
                CreatedAt = vibrationSensorData.CreatedAt
            };
        }
    }

    public class ThresholdResponse
    {
        public int Id { get; set; }
        public int DeviceId { get; set; }
        public string? DeviceName { get; set; }
        
        public double ThresholdVelocityX { get; set; }

        
        public string? MessageThresholdVelocityX { get; set; }

        public double ThresholdVelocityZ { get; set; }
        
        public string? MessageThresholdVelocityZ { get; set; }

        
        public double ThresholdVelocityY { get; set; }

        
        public string? MessageThresholdVelocityY { get; set; }

        public double ThresholdAccelerationX { get; set; }

        
        public string? MessageThresholdAccelerationX { get; set; }

        
        public double ThresholdAccelerationZ { get; set; }

       
        public string? MessageThresholdAccelerationZ { get; set; }

        
        public double ThresholdAccelerationY { get; set; }

        
        public string? MessageThresholdAccelerationY { get; set; }

        
        public double ThresholdTemperature { get; set; }

        
        public string? MessageThresholdTemperature { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
