using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReminderManager.Domain.Entities
{
    // MQTT Configuration
    public class MqttConfig
    {
        public string Host { get; set; } = "localhost";
        public int Port { get; set; } = 1883;
        public string Topic { get; set; } = "modbus/sensor/data";
        public string ClientId { get; set; } = "ModbusWorkerService";
        public string Username { get; set; }
        public string Password { get; set; }
        public int PublishIntervalMs { get; set; } = 5000;

        // Parse host to extract broker address (remove mqtt:// prefix if present)
        public string GetBrokerAddress()
        {
            if (string.IsNullOrEmpty(Host))
                return "localhost";

            var host = Host.Trim();
            if (host.StartsWith("mqtt://", StringComparison.OrdinalIgnoreCase))
                return host.Substring(7);
            if (host.StartsWith("mqtts://", StringComparison.OrdinalIgnoreCase))
                return host.Substring(8);

            return host;
        }
    }

    // MQTT Payload Models
    public class MqttDeviceData
    {
        public string DeviceName { get; set; }
        public double VelX { get; set; }
        public double ThresholdVelX { get; set; } = 100;

        public double VelY { get; set; }
        public double ThresholdVelY { get; set; } = 100;

        public double VelZ { get; set; }
        public double ThresholdVelZ { get; set; } = 100;
        public double AccX { get; set; }
        public double ThresholdAccX { get; set; } = 100;
        public double AccZ { get; set; }
        public double ThresholdAccZ { get; set; } = 100;

        public double AccY { get; set; }
        public double ThresholdAccY { get; set; } = 100;

        public double Temp { get; set; }
        public double ThresholdTemp { get; set; } = 100;
        public string Status { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class MqttPayload
    {
        public List<MqttDeviceData> Data { get; set; } = new();
        public DateTime SentAt { get; set; }
    }



}
