using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReminderManager.Domain.Entities;

namespace ReminderManager.Domain.DTO
{

    public class ThresholdEventFilterRequest
    {
        public int? DeviceId { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public bool Paginate { get; set; } = false;

        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 10;
    }


    public static class ThresholdEventMapper
    {
        public static ThresholdEventResponse ToThresholdEventResponse(this ThresholdEvent thresholdEvent)
        {
            if (thresholdEvent == null)
                return null!;

            return new ThresholdEventResponse
            {
                Id = thresholdEvent.Id,
                DeviceId = thresholdEvent.DeviceId,
                ColumnName = thresholdEvent.ColumnName,
                ActualValue = thresholdEvent.ActualValue,
                ThresholdValue = thresholdEvent.ThresholdValue,
                Message = thresholdEvent.Message,
                TriggeredAt = thresholdEvent.TriggeredAt
            };
        }
    }

    public class ThresholdEventResponse
    {
        public int Id { get; set; }
        public int? DeviceId { get; set; }
        public string ColumnName { get; set; }
        public double ActualValue { get; set; }
        public double ThresholdValue { get; set; }
        public string Message { get; set; }
        public DateTime TriggeredAt { get; set; }
    }
}
