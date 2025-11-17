using System.Data;
using Microsoft.EntityFrameworkCore;
using ReminderManager.Domain.Entities;

namespace ReminderManager.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
               .HasIndex(u => u.Username)
               .IsUnique();

            modelBuilder.Entity<ModbusDeviceConfig>()
               .HasIndex(u => u.DeviceName)
               .IsUnique();

            modelBuilder.Entity<VibrationSensorData>()
                .HasOne(v => v.Device)
                .WithMany(d => d.VibrationSensorDataList)
                .HasForeignKey(v => v.DeviceId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Threshold>()
                .HasOne(t => t.Device)
                .WithOne(d => d.Threshold)
                .HasForeignKey<Threshold>(t => t.DeviceId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<User>().HasData(
                new User {  Username = "Admin", Password = BCrypt.Net.BCrypt.HashPassword("123123") }
            );

        }

        public DbSet<User> User { get; set; }

        public DbSet<ModbusDeviceConfig> ModbusDeviceConfig { get; set; }

        public DbSet<VibrationSensorData> VibrationSensorData { get; set; }

        public DbSet<Threshold> Threshold { get; set; }

    }
}
