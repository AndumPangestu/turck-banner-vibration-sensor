using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReminderManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModbusTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("97798604-1719-49e5-bc3e-f86e3a36e6f7"));

            migrationBuilder.AlterColumn<int>(
                name: "device_id",
                table: "ModbusDeviceConfig",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "id", "password", "username" },
                values: new object[] { new Guid("8b7ddba6-a1b1-4309-b0aa-5d2dfecc305a"), "$2a$11$/wGOszXI1amy5rII2IN5YerD/iOlFPXXd87qh9eenBOdGm1Oo3tNO", "Admin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("8b7ddba6-a1b1-4309-b0aa-5d2dfecc305a"));

            migrationBuilder.AlterColumn<string>(
                name: "device_id",
                table: "ModbusDeviceConfig",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "id", "password", "username" },
                values: new object[] { new Guid("97798604-1719-49e5-bc3e-f86e3a36e6f7"), "$2a$11$85/3DbGYr8FRUujBIDRtzu6/Dzg/rdfGfv/GiuraaiWWpdVSuGyii", "Admin" });
        }
    }
}
