using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReminderManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateThresholdTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("541cb656-56c0-42b3-8972-cbfa3444ed76"));

            migrationBuilder.AlterColumn<string>(
                name: "message_threshold_temperature",
                table: "thresholds",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "message_threshold_acceleration_z",
                table: "thresholds",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "message_threshold_acceleration_x",
                table: "thresholds",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "id", "password", "username" },
                values: new object[] { new Guid("e1ff8bae-191d-4273-855a-6f9ff0e9aa01"), "$2a$11$LnfK8TJDqHOIaCFDfrdvz.U.M918xLcHLzen3bH4ViwwCd.Ndj0dC", "Admin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("e1ff8bae-191d-4273-855a-6f9ff0e9aa01"));

            migrationBuilder.AlterColumn<double>(
                name: "message_threshold_temperature",
                table: "thresholds",
                type: "double",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<double>(
                name: "message_threshold_acceleration_z",
                table: "thresholds",
                type: "double",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<double>(
                name: "message_threshold_acceleration_x",
                table: "thresholds",
                type: "double",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "id", "password", "username" },
                values: new object[] { new Guid("541cb656-56c0-42b3-8972-cbfa3444ed76"), "$2a$11$KfEK7wcdlXXXlIOkSGAqQORhi5uUejmr1plYoQPtLLt8u2lgMVtQW", "Admin" });
        }
    }
}
