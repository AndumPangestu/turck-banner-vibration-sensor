using System.Net;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using ReminderManager.Application.Exceptions;
using ReminderManager.Application.Interfaces;
using ReminderManager.Application.Validation;
using ReminderManager.Domain.DTO;
using ReminderManager.Domain.Entities;
using ReminderManager.Infrastructure.Data;
namespace ReminderManager.Infrastructure.Services
{
    public class ThresholdService: IThresholdService
    {
        private readonly AppDbContext _dbContext;

        public ThresholdService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Create
        public async Task<ThresholdResponse> Create(ThresholdRequest request)
        {
            var validator = new ThresholdValidator();
            var result = validator.Validate(request);
            if (!result.IsValid)
                throw new ValidationException(result.Errors);

            // Ensure device exists
            var device = await _dbContext.ModbusDeviceConfig
                .FirstOrDefaultAsync(d => d.DeviceId == request.DeviceId);

            if (device == null)
                throw new ResponseException(HttpStatusCode.NotFound, "Device not found");

            // Check one-to-one (threshold must be unique for each device)
            var existingThreshold = await _dbContext.Threshold
                .FirstOrDefaultAsync(t => t.DeviceId == request.DeviceId);

            if (existingThreshold != null)
                throw new ResponseException(HttpStatusCode.Conflict,
                    "Threshold for this device already exists");

            var threshold = new Threshold
            {
                DeviceId = request.DeviceId,
                ThresholdVelocityX = request.ThresholdVelocityX,
                MessageThresholdVelocityX = request.MessageThresholdVelocityX,
                ThresholdVelocityZ = request.ThresholdVelocityZ,
                MessageThresholdVelocityZ = request.MessageThresholdVelocityZ,
                ThresholdAccelerationX = request.ThresholdAccelerationX,
                MessageThresholdAccelerationX = request.MessageThresholdAccelerationX,
                ThresholdAccelerationZ = request.ThresholdAccelerationZ,
                MessageThresholdAccelerationZ = request.MessageThresholdAccelerationZ,
                ThresholdTemperature = request.ThresholdTemperature,
                MessageThresholdTemperature = request.MessageThresholdTemperature
            };

            threshold.Device = device;

            _dbContext.Threshold.Add(threshold);
            await _dbContext.SaveChangesAsync();

            return threshold.ToThresholdResponse();
        }


        // Get all
        public async Task<Pageable<ThresholdResponse>> Get(ThresholdFilterRequest filter)
        {
            var query = _dbContext.Threshold.Include(t => t.Device).AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Keyword))
            {
                // contoh: filter berdasarkan message
                query = query.Where(t =>
                    t.MessageThresholdVelocityX.Contains(filter.Keyword) ||
                    t.MessageThresholdVelocityZ.Contains(filter.Keyword) ||
                    t.MessageThresholdAccelerationX.Contains(filter.Keyword) ||
                    t.MessageThresholdAccelerationZ.Contains(filter.Keyword) ||
                    t.MessageThresholdTemperature.Contains(filter.Keyword)
                );
            }

            var total = await query.CountAsync();

            // order by created_at by default
            query = query.OrderByDescending(t => t.CreatedAt);

            List<Threshold> items;

            if (filter.Paginate)
            {
                var skip = (filter.Page - 1) * filter.Limit;
                items = await query.Skip(skip).Take(filter.Limit).ToListAsync();
            }
            else
            {
                items = await query.ToListAsync();
            }

            var response = new Pageable<ThresholdResponse>
            {
                Data = items.Select(x => x.ToThresholdResponse())
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



        // Show by Id
        public async Task<ThresholdResponse?> Show(int id)
        {
            var threshold = await _dbContext.Threshold
                .Include(t => t.Device)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (threshold == null)
            {
                throw new ResponseException(HttpStatusCode.NotFound, "Threshold not found");
            }

            return threshold.ToThresholdResponse();
        }



        // Update
        public async Task<ThresholdResponse> Update(int id, ThresholdRequest request)
        {
            var threshold = await _dbContext.Threshold
                 .Include(t => t.Device)
                 .FirstOrDefaultAsync(t => t.Id == id);
            if (threshold == null)
                throw new ResponseException(HttpStatusCode.NotFound, "Threshold not found");

            var validator = new ThresholdValidator();
            var result = validator.Validate(request);
            if (!result.IsValid)
                throw new ValidationException(result.Errors);

            // Jika DeviceId berubah → harus cek one-to-one rule
            if (threshold.DeviceId != request.DeviceId)
            {
                var deviceExists = await _dbContext.ModbusDeviceConfig
                    .AnyAsync(d => d.DeviceId == request.DeviceId);

                if (!deviceExists)
                    throw new ResponseException(HttpStatusCode.NotFound, "Device not found");

                var duplicate = await _dbContext.Threshold
                    .FirstOrDefaultAsync(t => t.DeviceId == request.DeviceId && t.Id != id);

                if (duplicate != null)
                    throw new ResponseException(HttpStatusCode.Conflict,
                        "Another threshold already exists for this device");

                threshold.DeviceId = request.DeviceId;
            }

            // update fields
            threshold.ThresholdVelocityX = request.ThresholdVelocityX;
            threshold.MessageThresholdVelocityX = request.MessageThresholdVelocityX;
            threshold.ThresholdVelocityZ = request.ThresholdVelocityZ;
            threshold.MessageThresholdVelocityZ = request.MessageThresholdVelocityZ;
            threshold.ThresholdAccelerationX = request.ThresholdAccelerationX;
            threshold.MessageThresholdAccelerationX = request.MessageThresholdAccelerationX;
            threshold.ThresholdAccelerationZ = request.ThresholdAccelerationZ;
            threshold.MessageThresholdAccelerationZ = request.MessageThresholdAccelerationZ;
            threshold.ThresholdTemperature = request.ThresholdTemperature;
            threshold.MessageThresholdTemperature = request.MessageThresholdTemperature;

            await _dbContext.SaveChangesAsync();

            return threshold.ToThresholdResponse();
        }


        // Delete
        public async Task<bool> Delete(int id)
        {
            var threshold = await _dbContext.Threshold.FindAsync(id);
            if (threshold == null)
            {
                throw new ResponseException(HttpStatusCode.NotFound, "Threshold not found");
            }

            _dbContext.Threshold.Remove(threshold);
            await _dbContext.SaveChangesAsync();
            return true;
        }

    }
}
