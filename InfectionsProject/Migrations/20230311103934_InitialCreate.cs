using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InfectionsProject.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Infections",
                columns: table => new
                {
                    InfectionId = table.Column<Guid>(type: "TEXT", nullable: false),
                    PatientIdentificationNumber = table.Column<long>(type: "INTEGER", nullable: false),
                    InfectionStatus = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Infections", x => x.InfectionId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Infections");
        }
    }
}
