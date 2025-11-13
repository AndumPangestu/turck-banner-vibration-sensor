//using System.Net;
//using FluentValidation.Results;
//using Microsoft.EntityFrameworkCore;
//using ReminderManager.Application.Exceptions;
//using ReminderManager.Application.Interfaces;
//using ReminderManager.Application.Validation;
//using ReminderManager.Domain.DTO;
//using ReminderManager.Domain.Entities;
//using ReminderManager.Infrastructure.Data;

//namespace ReminderManager.Infrastructure.Services
//{
//    public class RackService : IRackService
//    {
//        private readonly AppDbContext _dbContext;

//        public RackService(AppDbContext dbContext)
//        {
//            _dbContext = dbContext;
//        }

//        // Create
//        public async Task<RackResponse> Create(RackRequest request)
//        {
//            var validator = new RackValidator();
//            ValidationResult result = validator.Validate(request);
//            if (!result.IsValid)
//            {
//                throw new ValidationException(result.Errors);
//            }

//            var existingCode = await _dbContext.Rack
//                .FirstOrDefaultAsync(d => d.Code == request.Code);

//            if (existingCode != null)
//            {
//                throw new ResponseException(HttpStatusCode.Conflict, "Code is already in use");
//            }

//            var rack = new Rack
//            {
//                Code = request.Code,
//                CreatedAt = DateTime.UtcNow,
//                UpdatedAt = DateTime.UtcNow
//            };

//            _dbContext.Rack.Add(rack);
//            await _dbContext.SaveChangesAsync();

//            return rack.ToRackResponse();
//        }

//        // Get all
//        public async Task<Pageable<RackResponse>> Get(RackFilterRequest filter)
//        {
//            // query awal
//            var query = _dbContext.Rack.AsQueryable();

//            // filter keyword (Code LIKE %keyword%)
//            if (!string.IsNullOrWhiteSpace(filter.Keyword))
//            {
//                query = query.Where(r => r.Code.Contains(filter.Keyword));
//            }

//            // total sebelum pagination
//            var total = await query.CountAsync();

//            // order default by Code asc
//            query = query.OrderBy(r => r.Code);

//            List<Rack> racks;

//            if (filter.Paginate)
//            {
//                var skip = (filter.Page - 1) * filter.Limit;
//                racks = await query.Skip(skip).Take(filter.Limit).ToListAsync();
//            }
//            else
//            {
//                racks = await query.ToListAsync();
//            }

//            var response = new Pageable<RackResponse>
//            {
//                Data = racks.Select(r => r.ToRackResponse())
//            };

//            if (filter.Paginate)
//            {
//                response.Pagination = new Pagination
//                {
//                    CurrPage = filter.Page,
//                    TotalPage = (int)Math.Ceiling(total / (double)filter.Limit),
//                    Limit = filter.Limit,
//                    Total = total
//                };
//            }

//            return response;
//        }


//        // Show by Id
//        public async Task<RackResponse?> Show(int id)
//        {
//            var rack = await _dbContext.Rack.FindAsync(id);
//            if (rack == null)
//            {
//                throw new ResponseException(HttpStatusCode.NotFound, "Rack not found");
//            }

//            return rack.ToRackResponse();
//        }

//        // Update
//        public async Task<RackResponse> Update(int id, RackRequest request)
//        {
//            var rack = await _dbContext.Rack.FindAsync(id);
//            if (rack == null)
//            {
//                throw new ResponseException(HttpStatusCode.NotFound, "Rack not found");
//            }

//            var validator = new RackValidator();
//            ValidationResult result = validator.Validate(request);
//            if (!result.IsValid)
//            {
//                throw new ValidationException(result.Errors);
//            }

//            // check duplicate code (exclude itself)
//            var existingCode = await _dbContext.Rack
//                .FirstOrDefaultAsync(d => d.Code == request.Code && d.Id != id);
//            if (existingCode != null)
//            {
//                throw new ResponseException(HttpStatusCode.Conflict, "Code is already in use");
//            }

//            rack.Code = request.Code;
//            rack.UpdatedAt = DateTime.UtcNow;

//            _dbContext.Rack.Update(rack);
//            await _dbContext.SaveChangesAsync();

//            return rack.ToRackResponse();
//        }

//        // Delete
//        public async Task<bool> Delete(int id)
//        {
//            var rack = await _dbContext.Rack.FindAsync(id);
//            if (rack == null)
//            {
//                throw new ResponseException(HttpStatusCode.NotFound, "Rack not found");
//            }

//            _dbContext.Rack.Remove(rack);
//            await _dbContext.SaveChangesAsync();
//            return true;
//        }
//    }
//}
