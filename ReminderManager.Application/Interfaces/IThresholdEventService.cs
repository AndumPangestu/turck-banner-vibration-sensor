using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReminderManager.Domain.DTO;

namespace ReminderManager.Application.Interfaces
{
    public interface IThresholdEventService
    {
        Task<Pageable<ThresholdEventResponse>> Get(ThresholdEventFilterRequest filter);
    }
}
