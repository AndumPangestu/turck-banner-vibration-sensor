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
    public class ThresholdEventService : IThresholdEventService
    {

        private readonly AppDbContext _dbContext;

        public ThresholdEventService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public async Task<Pageable<ThresholdEventResponse>> Get(ThresholdEventFilterRequest filter)
        {

            // query awal
            var query = _dbContext.ThresholdEvent.Include(t => t.Device).AsQueryable();

            // filter keyword (Code LIKE %keyword%)
            if (filter.DeviceId.HasValue)
            {
                query = query.Where(v => v.DeviceId == filter.DeviceId.Value);
            }

            if (filter.StartDate.HasValue)
            {
                query = query.Where(v => v.TriggeredAt >= filter.StartDate.Value);
            }

            if (filter.EndDate.HasValue)
            {
                query = query.Where(v => v.TriggeredAt <= filter.EndDate.Value);
            }

            // total sebelum pagination
            var total = await query.CountAsync();

            // order default by Code asc
            query = query.OrderByDescending(r => r.TriggeredAt);


            List<ThresholdEvent> vibrationSensorData;

            if (filter.Paginate)
            {
                var skip = (filter.Page - 1) * filter.Limit;
                vibrationSensorData = await query.Skip(skip).Take(filter.Limit).ToListAsync();
            }
            else
            {
                vibrationSensorData = await query.ToListAsync();
            }

            var response = new Pageable<ThresholdEventResponse>
            {
                Data = vibrationSensorData.Select(r => r.ToThresholdEventResponse())
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
