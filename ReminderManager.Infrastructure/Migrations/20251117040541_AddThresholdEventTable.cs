using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReminderManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddThresholdEventTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("b365e9f8-f2e6-438f-87ca-40bfc946dd70"));

            migrationBuilder.CreateTable(
                name: "threshold_events",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    device_id = table.Column<int>(type: "int", nullable: true),
                    column_name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    actual_value = table.Column<double>(type: "double", nullable: false),
                    threshold_value = table.Column<double>(type: "double", nullable: false),
                    message = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_threshold_events", x => x.id);
                    table.ForeignKey(
                        name: "FK_threshold_events_modbus_device_configs_device_id",
                        column: x => x.device_id,
                        principalTable: "modbus_device_configs",
                        principalColumn: "device_id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "id", "password", "username" },
                values: new object[] { new Guid("5368b86c-130d-4fd5-8468-9656eb967579"), "$2a$11$uO2LRd5T9a/UOuVjryCufOACrklZngjHOtcyq3E8aduLhxh0h3wCm", "Admin" });

            migrationBuilder.CreateIndex(
                name: "IX_threshold_events_device_id",
                table: "threshold_events",
                column: "device_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "threshold_events");

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("5368b86c-130d-4fd5-8468-9656eb967579"));

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "id", "password", "username" },
                values: new object[] { new Guid("b365e9f8-f2e6-438f-87ca-40bfc946dd70"), "$2a$11$tmDG9gaxobkYT5G9LU1e9ecVqgZ8sUkGMfyC1W22XrYeeVVjFr6WO", "Admin" });
        }
    }
}
