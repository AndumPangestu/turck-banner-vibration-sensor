using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReminderManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateThresholdEventTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("5368b86c-130d-4fd5-8468-9656eb967579"));

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "threshold_events",
                newName: "triggered_at");

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "id", "password", "username" },
                values: new object[] { new Guid("b79f9373-9f8f-4b53-9808-af6ccccb0a7d"), "$2a$11$tz2zmlvJvuzNv7Dr4PS2/ehPGvqOUIs.7D8oLUjaH/MV.hhvWw2RC", "Admin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("b79f9373-9f8f-4b53-9808-af6ccccb0a7d"));

            migrationBuilder.RenameColumn(
                name: "triggered_at",
                table: "threshold_events",
                newName: "created_at");

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "id", "password", "username" },
                values: new object[] { new Guid("5368b86c-130d-4fd5-8468-9656eb967579"), "$2a$11$uO2LRd5T9a/UOuVjryCufOACrklZngjHOtcyq3E8aduLhxh0h3wCm", "Admin" });
        }
    }
}
