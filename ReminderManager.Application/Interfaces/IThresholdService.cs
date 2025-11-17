using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReminderManager.Domain.DTO;

namespace ReminderManager.Application.Interfaces
{
    public interface IThresholdService
    {
        // Create
        Task<ThresholdResponse> Create(ThresholdRequest request);

        // Get all
        Task<Pageable<ThresholdResponse>> Get(ThresholdFilterRequest filter);

        // Get by Id (show detail)
        Task<ThresholdResponse?> Show(int id);

        // Update
        Task<ThresholdResponse> Update(int id, ThresholdRequest request);

        // Delete
        Task<bool> Delete(int id);
    }
}
