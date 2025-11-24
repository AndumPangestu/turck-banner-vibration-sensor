using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReminderManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModbusTableToRTU : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("b79f9373-9f8f-4b53-9808-af6ccccb0a7d"));

            migrationBuilder.RenameColumn(
                name: "port",
                table: "modbus_device_configs",
                newName: "stop_bits");

            migrationBuilder.RenameColumn(
                name: "ip_address",
                table: "modbus_device_configs",
                newName: "port_name");

            migrationBuilder.AddColumn<double>(
                name: "acceleration_y",
                table: "vibration_sensor_data",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "velocity_y",
                table: "vibration_sensor_data",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "message_threshold_acceleration_y",
                table: "thresholds",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "message_threshold_velocity_y",
                table: "thresholds",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<double>(
                name: "threshold_acceleration_y",
                table: "thresholds",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "threshold_velocity_y",
                table: "thresholds",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "baud_rate",
                table: "modbus_device_configs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "data_bits",
                table: "modbus_device_configs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "parity",
                table: "modbus_device_configs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "id", "password", "username" },
                values: new object[] { new Guid("52c566f5-6a4d-4e8e-a9b7-e1ecf68b4bb2"), "$2a$11$DhC05JvSZVy2fzH83XRRg.ziniXJqcdMl3YxQ4HlGE9jZqZPdrhaS", "Admin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("52c566f5-6a4d-4e8e-a9b7-e1ecf68b4bb2"));

            migrationBuilder.DropColumn(
                name: "acceleration_y",
                table: "vibration_sensor_data");

            migrationBuilder.DropColumn(
                name: "velocity_y",
                table: "vibration_sensor_data");

            migrationBuilder.DropColumn(
                name: "message_threshold_acceleration_y",
                table: "thresholds");

            migrationBuilder.DropColumn(
                name: "message_threshold_velocity_y",
                table: "thresholds");

            migrationBuilder.DropColumn(
                name: "threshold_acceleration_y",
                table: "thresholds");

            migrationBuilder.DropColumn(
                name: "threshold_velocity_y",
                table: "thresholds");

            migrationBuilder.DropColumn(
                name: "baud_rate",
                table: "modbus_device_configs");

            migrationBuilder.DropColumn(
                name: "data_bits",
                table: "modbus_device_configs");

            migrationBuilder.DropColumn(
                name: "parity",
                table: "modbus_device_configs");

            migrationBuilder.RenameColumn(
                name: "stop_bits",
                table: "modbus_device_configs",
                newName: "port");

            migrationBuilder.RenameColumn(
                name: "port_name",
                table: "modbus_device_configs",
                newName: "ip_address");

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "id", "password", "username" },
                values: new object[] { new Guid("b79f9373-9f8f-4b53-9808-af6ccccb0a7d"), "$2a$11$tz2zmlvJvuzNv7Dr4PS2/ehPGvqOUIs.7D8oLUjaH/MV.hhvWw2RC", "Admin" });
        }
    }
}
