using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReminderManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddVibartionSensorDataTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("394ad5fe-9f8b-44d0-a90e-08b6f5c9fb84"));

            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "modbus_device_configs",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "vibration_sensor_data",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    device_id = table.Column<int>(type: "int", nullable: true),
                    velocity_x = table.Column<double>(type: "double", nullable: false),
                    velocity_z = table.Column<double>(type: "double", nullable: false),
                    acceleration_x = table.Column<double>(type: "double", nullable: false),
                    acceleration_z = table.Column<double>(type: "double", nullable: false),
                    temperature = table.Column<double>(type: "double", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vibration_sensor_data", x => x.id);
                    table.ForeignKey(
                        name: "FK_vibration_sensor_data_modbus_device_configs_device_id",
                        column: x => x.device_id,
                        principalTable: "modbus_device_configs",
                        principalColumn: "device_id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "id", "password", "username" },
                values: new object[] { new Guid("b405645c-b2ec-4010-b190-f77e05041833"), "$2a$11$RH/JqjqdZA/flwANjCBpFeyzq1ufxlMr9SqMAI3ziJFSbZN6.Wfj6", "Admin" });

            migrationBuilder.CreateIndex(
                name: "IX_vibration_sensor_data_device_id",
                table: "vibration_sensor_data",
                column: "device_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "vibration_sensor_data");

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("b405645c-b2ec-4010-b190-f77e05041833"));

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "modbus_device_configs");

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "id", "password", "username" },
                values: new object[] { new Guid("394ad5fe-9f8b-44d0-a90e-08b6f5c9fb84"), "$2a$11$EW83uhzbpehM/Ol7d/GiounkmVLjfzR7XOk5pSQuCpJbU6AgPndD.", "Admin" });
        }
    }
}
