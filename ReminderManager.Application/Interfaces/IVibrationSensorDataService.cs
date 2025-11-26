using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReminderManager.Domain.DTO;

namespace ReminderManager.Application.Interfaces
{
    public interface IVibrationSensorDataService
    {
        Task<Pageable<VibrationSensorDataResponse>> GetVibrationDataAsync(VibrationSensorDataFilterRequest filter);
        Task<string> AnalyzeData(VibrationSensorDataFilterRequest filter);
    }
}
