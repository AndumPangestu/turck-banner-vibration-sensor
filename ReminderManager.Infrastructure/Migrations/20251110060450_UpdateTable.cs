using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReminderManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmailSender");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ModbusDeviceConfig",
                table: "ModbusDeviceConfig");

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("8b7ddba6-a1b1-4309-b0aa-5d2dfecc305a"));

            migrationBuilder.RenameTable(
                name: "ModbusDeviceConfig",
                newName: "modbus_device_configs");

            migrationBuilder.AlterColumn<string>(
                name: "device_name",
                table: "modbus_device_configs",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddPrimaryKey(
                name: "PK_modbus_device_configs",
                table: "modbus_device_configs",
                column: "device_id");

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "id", "password", "username" },
                values: new object[] { new Guid("394ad5fe-9f8b-44d0-a90e-08b6f5c9fb84"), "$2a$11$EW83uhzbpehM/Ol7d/GiounkmVLjfzR7XOk5pSQuCpJbU6AgPndD.", "Admin" });

            migrationBuilder.CreateIndex(
                name: "IX_modbus_device_configs_device_name",
                table: "modbus_device_configs",
                column: "device_name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_modbus_device_configs",
                table: "modbus_device_configs");

            migrationBuilder.DropIndex(
                name: "IX_modbus_device_configs_device_name",
                table: "modbus_device_configs");

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("394ad5fe-9f8b-44d0-a90e-08b6f5c9fb84"));

            migrationBuilder.RenameTable(
                name: "modbus_device_configs",
                newName: "ModbusDeviceConfig");

            migrationBuilder.AlterColumn<string>(
                name: "device_name",
                table: "ModbusDeviceConfig",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ModbusDeviceConfig",
                table: "ModbusDeviceConfig",
                column: "device_id");

            migrationBuilder.CreateTable(
                name: "EmailSender",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    email = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    password = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailSender", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "id", "password", "username" },
                values: new object[] { new Guid("8b7ddba6-a1b1-4309-b0aa-5d2dfecc305a"), "$2a$11$/wGOszXI1amy5rII2IN5YerD/iOlFPXXd87qh9eenBOdGm1Oo3tNO", "Admin" });

            migrationBuilder.CreateIndex(
                name: "IX_EmailSender_email",
                table: "EmailSender",
                column: "email",
                unique: true);
        }
    }
}
