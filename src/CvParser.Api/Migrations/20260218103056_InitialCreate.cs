using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CvParser.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApplicationSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LlmFallbackOnly = table.Column<bool>(type: "boolean", nullable: false),
                    LlmModel = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DateOfBirth = table.Column<DateOnly>(type: "date", nullable: false),
                    DepartmentName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Skills = table.Column<List<string>>(type: "text[]", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeProfiles", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "ApplicationSettings",
                columns: new[] { "LlmFallbackOnly", "LlmModel", "UpdatedAt" },
                values: new object[] { false, "llama-3.3-70b-versatile", DateTime.UtcNow });

            migrationBuilder.InsertData(
                table: "EmployeeProfiles",
                columns: new[] { "Id", "FirstName", "LastName", "DateOfBirth", "DepartmentName", "Skills" },
                values: new object[,]
                {
                    { Guid.Parse("7e06c2fb-7c26-4d36-9f5a-49f5e7e78010"), "Mateo", "Silva", new DateOnly(1996, 11, 3), "Engineering", new[] { "TypeScript", "API design", "React" } },
                    { Guid.Parse("63a9d4fb-6a52-4a2c-a0a6-3dfe4b5c7f12"), "Nia", "Okoye", new DateOnly(1999, 7, 19), "Data", new[] { "SQL", "Python" } },
                    { Guid.Parse("b824a2f3-cb16-4a59-bc80-7c00f18c9f5d"), "Liam", "Keller", new DateOnly(1995, 2, 6), "Design", new[] { "UX research", "Figma" } },
                    { Guid.Parse("593ea1a1-d5b2-4f59-9c4c-fb9c424f2d23"), "Priya", "Singh", new DateOnly(1998, 5, 30), "Marketing", new[] { "Content strategy", "Campaign planning" } }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "ApplicationSettings");
            migrationBuilder.DropTable(name: "EmployeeProfiles");
        }
    }
}
