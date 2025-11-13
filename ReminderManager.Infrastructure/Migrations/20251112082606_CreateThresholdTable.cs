using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReminderManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CreateThresholdTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("b405645c-b2ec-4010-b190-f77e05041833"));

            migrationBuilder.CreateTable(
                name: "thresholds",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    device_id = table.Column<int>(type: "int", nullable: true),
                    threshold_velocity_x = table.Column<double>(type: "double", nullable: false),
                    message_threshold_velocity_x = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    threshold_velocity_z = table.Column<double>(type: "double", nullable: false),
                    message_threshold_velocity_z = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    threshold_acceleration_x = table.Column<double>(type: "double", nullable: false),
                    message_threshold_acceleration_x = table.Column<double>(type: "double", nullable: false),
                    threshold_acceleration_z = table.Column<double>(type: "double", nullable: false),
                    message_threshold_acceleration_z = table.Column<double>(type: "double", nullable: false),
                    threshold_temperature = table.Column<double>(type: "double", nullable: false),
                    message_threshold_temperature = table.Column<double>(type: "double", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_thresholds", x => x.id);
                    table.ForeignKey(
                        name: "FK_thresholds_modbus_device_configs_device_id",
                        column: x => x.device_id,
                        principalTable: "modbus_device_configs",
                        principalColumn: "device_id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "id", "password", "username" },
                values: new object[] { new Guid("541cb656-56c0-42b3-8972-cbfa3444ed76"), "$2a$11$KfEK7wcdlXXXlIOkSGAqQORhi5uUejmr1plYoQPtLLt8u2lgMVtQW", "Admin" });

            migrationBuilder.CreateIndex(
                name: "IX_thresholds_device_id",
                table: "thresholds",
                column: "device_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "thresholds");

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("541cb656-56c0-42b3-8972-cbfa3444ed76"));

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "id", "password", "username" },
                values: new object[] { new Guid("b405645c-b2ec-4010-b190-f77e05041833"), "$2a$11$RH/JqjqdZA/flwANjCBpFeyzq1ufxlMr9SqMAI3ziJFSbZN6.Wfj6", "Admin" });
        }
    }
}
