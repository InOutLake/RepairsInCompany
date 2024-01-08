using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RepairsInCompany.Migrations.RepairsDb
{
    /// <inheritdoc />
    public partial class InitialMigrationRepairsDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Equipment",
                columns: table => new
                {
                    EquipmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    Name = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Unique_Identifier1", x => x.EquipmentId);
                });

            migrationBuilder.CreateTable(
                name: "Registration",
                columns: table => new
                {
                    StartDateTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    EquipmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PeriodInDays = table.Column<int>(type: "int", nullable: false),
                    EndDate = table.Column<DateTime>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Unique_Identifier2", x => new { x.StartDateTime, x.EquipmentId });
                    table.ForeignKey(
                        name: "Registered",
                        column: x => x.EquipmentId,
                        principalTable: "Equipment",
                        principalColumn: "EquipmentId");
                });

            migrationBuilder.CreateTable(
                name: "Repair",
                columns: table => new
                {
                    StartDateTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    EquipmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EndDateTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    Planned = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Unique_Identifier3", x => new { x.StartDateTime, x.EquipmentId });
                    table.ForeignKey(
                        name: "Being Repaired",
                        column: x => x.EquipmentId,
                        principalTable: "Equipment",
                        principalColumn: "EquipmentId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Registration_EquipmentId",
                table: "Registration",
                column: "EquipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Repair_EquipmentId",
                table: "Repair",
                column: "EquipmentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Registration");

            migrationBuilder.DropTable(
                name: "Repair");

            migrationBuilder.DropTable(
                name: "Equipment");
        }
    }
}
