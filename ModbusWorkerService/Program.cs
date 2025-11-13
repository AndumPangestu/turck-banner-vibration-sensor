using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModbusTcpWorkerService;
using ReminderManager.Domain.Entities;

public class Program
{
    public static async Task Main(string[] args)
    {
        try
        {
            var host = CreateHostBuilder(args).Build();
            Console.WriteLine("=== Modbus TCP Worker Service with MQTT ===");
            Console.WriteLine("Starting application...\n");
            await host.RunAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fatal error: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
            throw;
        }
    }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, config) =>
            {
                // Add additional configuration sources if needed
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                config.AddJsonFile(
                    $"appsettings.{context.HostingEnvironment.EnvironmentName}.json",
                    optional: true,
                    reloadOnChange: true
                );
                config.AddEnvironmentVariables();
                config.AddCommandLine(args);
            })
            .ConfigureServices((context, services) =>
            {
                // Get connection string
                var connectionString = context.Configuration.GetConnectionString("DefaultConnection");
                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new InvalidOperationException(
                        "Connection string 'DefaultConnection' not found in configuration");
                }

                // Load MQTT Configuration from appsettings.json
                var mqttConfig = new MqttConfig();
                context.Configuration.GetSection("Mqtt").Bind(mqttConfig);

                // Validate MQTT configuration
                if (string.IsNullOrEmpty(mqttConfig.Host))
                {
                    Console.WriteLine("WARNING: MQTT Host not configured, using default 'localhost'");
                    mqttConfig.Host = "localhost";
                }

                if (string.IsNullOrEmpty(mqttConfig.Topic))
                {
                    Console.WriteLine("WARNING: MQTT Topic not configured, using default 'modbus/sensor/data'");
                    mqttConfig.Topic = "modbus/sensor/data";
                }

                // Log MQTT configuration
                Console.WriteLine("\n=== MQTT Configuration ===");
                Console.WriteLine($"Host: {mqttConfig.Host}");
                Console.WriteLine($"Port: {mqttConfig.Port}");
                Console.WriteLine($"Topic: {mqttConfig.Topic}");
                Console.WriteLine($"Client ID: {mqttConfig.ClientId}");
                Console.WriteLine($"Publish Interval: {mqttConfig.PublishIntervalMs}ms");
                Console.WriteLine($"Username: {(string.IsNullOrEmpty(mqttConfig.Username) ? "Not configured" : "Configured")}");
                Console.WriteLine("==========================\n");

                // Register all Modbus services with MQTT configuration
                services.AddModbusServices(connectionString, mqttConfig);
            })
            .ConfigureLogging((context, logging) =>
            {
                logging.ClearProviders();
                logging.AddConsole();

                // Set log levels from configuration or use defaults
                var logLevel = context.Configuration.GetValue<LogLevel?>("Logging:LogLevel:Default")
                    ?? LogLevel.Information;
                logging.SetMinimumLevel(logLevel);

                // Optional: Add more detailed logging for specific namespaces
                logging.AddFilter("Microsoft.EntityFrameworkCore", LogLevel.Warning);
                logging.AddFilter("ModbusTcpWorkerService", LogLevel.Information);
            })
            .UseConsoleLifetime(); // Allows Ctrl+C to gracefully shutdown
}