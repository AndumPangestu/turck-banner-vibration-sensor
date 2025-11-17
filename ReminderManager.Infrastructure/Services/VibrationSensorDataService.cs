using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReminderManager.Application.Interfaces;
using ReminderManager.Domain.DTO;
using ReminderManager.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using ReminderManager.Domain.Entities;

namespace ReminderManager.Infrastructure.Services
{
    public class VibrationSensorDataService : IVibrationSensorDataService
    {

        private readonly AppDbContext _dbContext;

        public VibrationSensorDataService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public async Task<Pageable<VibrationSensorDataResponse>> GetVibrationDataAsync(VibrationSensorDataFilterRequest filter)
        {

            // query awal
            var query = _dbContext.VibrationSensorData.Include(t => t.Device).AsQueryable();

            // filter keyword (Code LIKE %keyword%)
            if (filter.DeviceId.HasValue)
            {
                query = query.Where(v => v.DeviceId == filter.DeviceId.Value);
            }

            if (filter.StartDate.HasValue)
            {
                query = query.Where(v => v.CreatedAt >= filter.StartDate.Value);
            }

            if (filter.EndDate.HasValue)
            {
                query = query.Where(v => v.CreatedAt <= filter.EndDate.Value);
            }

            // total sebelum pagination
            var total = await query.CountAsync();

            // order default by Code asc
            query = query.OrderByDescending(r => r.CreatedAt);


            List<VibrationSensorData> vibrationSensorData;

            if (filter.Paginate)
            {
                var skip = (filter.Page - 1) * filter.Limit;
                vibrationSensorData = await query.Skip(skip).Take(filter.Limit).ToListAsync();
            }
            else
            {
                vibrationSensorData = await query.ToListAsync();
            }

            var response = new Pageable<VibrationSensorDataResponse>
            {
                Data = vibrationSensorData.Select(r => r.ToVibrationSensorDataResponse())
            };

            if (filter.Paginate)
            {
                response.Pagination = new Pagination
                {
                    CurrPage = filter.Page,
                    TotalPage = (int)Math.Ceiling(total / (double)filter.Limit),
                    Limit = filter.Limit,
                    Total = total
                };
            }

            return response;

        }
    }
}
