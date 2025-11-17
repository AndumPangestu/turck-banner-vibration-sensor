using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReminderManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateThresholdRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_thresholds_modbus_device_configs_device_id",
                table: "thresholds");

            migrationBuilder.DropIndex(
                name: "IX_thresholds_device_id",
                table: "thresholds");

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("e1ff8bae-191d-4273-855a-6f9ff0e9aa01"));

            migrationBuilder.AlterColumn<string>(
                name: "message_threshold_velocity_z",
                table: "thresholds",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "message_threshold_velocity_x",
                table: "thresholds",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "message_threshold_temperature",
                table: "thresholds",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "message_threshold_acceleration_z",
                table: "thresholds",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "message_threshold_acceleration_x",
                table: "thresholds",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "device_id",
                table: "thresholds",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "id", "password", "username" },
                values: new object[] { new Guid("b365e9f8-f2e6-438f-87ca-40bfc946dd70"), "$2a$11$tmDG9gaxobkYT5G9LU1e9ecVqgZ8sUkGMfyC1W22XrYeeVVjFr6WO", "Admin" });

            migrationBuilder.CreateIndex(
                name: "IX_thresholds_device_id",
                table: "thresholds",
                column: "device_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_thresholds_modbus_device_configs_device_id",
                table: "thresholds",
                column: "device_id",
                principalTable: "modbus_device_configs",
                principalColumn: "device_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_thresholds_modbus_device_configs_device_id",
                table: "thresholds");

            migrationBuilder.DropIndex(
                name: "IX_thresholds_device_id",
                table: "thresholds");

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("b365e9f8-f2e6-438f-87ca-40bfc946dd70"));

            migrationBuilder.UpdateData(
                table: "thresholds",
                keyColumn: "message_threshold_velocity_z",
                keyValue: null,
                column: "message_threshold_velocity_z",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "message_threshold_velocity_z",
                table: "thresholds",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "thresholds",
                keyColumn: "message_threshold_velocity_x",
                keyValue: null,
                column: "message_threshold_velocity_x",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "message_threshold_velocity_x",
                table: "thresholds",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "thresholds",
                keyColumn: "message_threshold_temperature",
                keyValue: null,
                column: "message_threshold_temperature",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "message_threshold_temperature",
                table: "thresholds",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "thresholds",
                keyColumn: "message_threshold_acceleration_z",
                keyValue: null,
                column: "message_threshold_acceleration_z",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "message_threshold_acceleration_z",
                table: "thresholds",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "thresholds",
                keyColumn: "message_threshold_acceleration_x",
                keyValue: null,
                column: "message_threshold_acceleration_x",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "message_threshold_acceleration_x",
                table: "thresholds",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "device_id",
                table: "thresholds",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "id", "password", "username" },
                values: new object[] { new Guid("e1ff8bae-191d-4273-855a-6f9ff0e9aa01"), "$2a$11$LnfK8TJDqHOIaCFDfrdvz.U.M918xLcHLzen3bH4ViwwCd.Ndj0dC", "Admin" });

            migrationBuilder.CreateIndex(
                name: "IX_thresholds_device_id",
                table: "thresholds",
                column: "device_id");

            migrationBuilder.AddForeignKey(
                name: "FK_thresholds_modbus_device_configs_device_id",
                table: "thresholds",
                column: "device_id",
                principalTable: "modbus_device_configs",
                principalColumn: "device_id");
        }
    }
}
