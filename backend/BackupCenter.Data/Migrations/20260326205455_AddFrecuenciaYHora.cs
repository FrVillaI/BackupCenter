using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackupCenter.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFrecuenciaYHora : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Frecuencia",
                table: "Empresas");

            migrationBuilder.AddColumn<int>(
                name: "FrecuenciaHoras",
                table: "Empresas",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "FechaCreacion", "PasswordHash" },
                values: new object[] { new DateTime(2026, 3, 26, 20, 54, 54, 628, DateTimeKind.Utc).AddTicks(8249), "$2a$11$nnd3.9laW40w0GlIeQajDugHcthYyoJ.aNl5/i.pJhEat7Iwp41NS" });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "FechaCreacion", "PasswordHash" },
                values: new object[] { new DateTime(2026, 3, 26, 20, 54, 54, 857, DateTimeKind.Utc).AddTicks(956), "$2a$11$np3wihGvEWMEMjjnbMXyP.CkFRVsMv5QFSXJMGhwbjG.PwVx85jG." });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FrecuenciaHoras",
                table: "Empresas");

            migrationBuilder.AddColumn<string>(
                name: "Frecuencia",
                table: "Empresas",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "FechaCreacion", "PasswordHash" },
                values: new object[] { new DateTime(2026, 3, 19, 21, 48, 48, 474, DateTimeKind.Utc).AddTicks(2272), "$2a$11$0Q3uyedwzY.kqVIAqzEFIeyAZuccbKr79Sif3g4UDfhKWMO2GVzqG" });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "FechaCreacion", "PasswordHash" },
                values: new object[] { new DateTime(2026, 3, 19, 21, 48, 48, 696, DateTimeKind.Utc).AddTicks(9074), "$2a$11$RrSlBluTjAANBknGdaM2FexysvtW3C3nLnnN889uUgK.AMteT07g." });
        }
    }
}
