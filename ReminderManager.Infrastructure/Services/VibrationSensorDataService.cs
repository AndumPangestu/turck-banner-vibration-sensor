using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ReminderManager.Application.Interfaces;
using ReminderManager.Domain.DTO;
using ReminderManager.Domain.Entities;
using ReminderManager.Infrastructure.Data;

namespace ReminderManager.Infrastructure.Services
{
    public class VibrationSensorDataService : IVibrationSensorDataService
    {

        private readonly AppDbContext _dbContext;
        private readonly HttpClient _httpClient;

        public VibrationSensorDataService(AppDbContext dbContext, HttpClient httpClient)
        {
            _dbContext = dbContext;
            _httpClient = httpClient;
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

        public async Task<string> AnalyzeData(VibrationSensorDataFilterRequest filter)
        {
            // Query awal
            var query = _dbContext.VibrationSensorData
                .Include(t => t.Device)
                .AsQueryable();

            // Filter
            if (filter.DeviceId.HasValue)
                query = query.Where(v => v.DeviceId == filter.DeviceId.Value);

            if (filter.StartDate.HasValue)
                query = query.Where(v => v.CreatedAt >= filter.StartDate.Value);

            if (filter.EndDate.HasValue)
                query = query.Where(v => v.CreatedAt <= filter.EndDate.Value);

            // Ambil 100 data terakhir
            var vibrationSensorData = await query
                .OrderByDescending(r => r.CreatedAt)
                .Take(100)
                .ToListAsync();

            var dtoList = vibrationSensorData.Select(v => new VibrationSensorDataAnalyzeRequest
            {
                VelocityX = v.VelocityX,
                VelocityY = v.VelocityY,
                VelocityZ = v.VelocityZ,

                AccelerationX = v.AccelerationX,
                AccelerationY = v.AccelerationY,
                AccelerationZ = v.AccelerationZ,

                Temperature = v.Temperature,
                CreatedAt = v.CreatedAt
            }).ToList();

            // ------------------------------
            // JSON Body
            // ------------------------------
            var body = new { data = dtoList };

            


            string jsonBody = JsonSerializer.Serialize(
                body,
                new JsonSerializerOptions { WriteIndented = true }
            );

            Console.WriteLine("=== Sending Body to n8n ===");
            Console.WriteLine(jsonBody);


            // ------------------------------
            // Kirim POST
            // ------------------------------
            try
            {
                var url = "https://n8n.robotix-id.cloud/webhook/ai-tohotec";
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(url, content);

                Console.WriteLine("Response status: " + response.StatusCode);

                var responseText = await response.Content.ReadAsStringAsync();

                // Parse JSON array → ambil objek pertama → ambil field Ai_Said
                using var doc = JsonDocument.Parse(responseText);

                string aiSaid = doc.RootElement[0].GetProperty("Ai_Said").GetString();

                // return hanya AI_Said
                return aiSaid;

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while calling n8n: " + ex.Message);
                return $"Error: {ex.Message}";
            }
        }



    }
}
