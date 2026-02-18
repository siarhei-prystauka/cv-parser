using System;
using Microsoft.EntityFrameworkCore.Migrations;

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
                    Key = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationSettings", x => x.Key);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DateOfBirth = table.Column<DateOnly>(type: "date", nullable: false),
                    DepartmentName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Skills = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeProfiles", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationSettings_Key",
                table: "ApplicationSettings",
                column: "Key");

            migrationBuilder.InsertData(
                table: "EmployeeProfiles",
                columns: new[] { "Id", "FirstName", "LastName", "DateOfBirth", "DepartmentName", "Skills" },
                values: new object[,]
                {
                    { Guid.Parse("2b4cc3a4-90d3-4e8b-8c16-8c50f6c5f9f1"), "Aya", "Mori", new DateOnly(1998, 4, 12), "Product", "[\"Product discovery\",\"Roadmapping\"]" },
                    { Guid.Parse("7e06c2fb-7c26-4d36-9f5a-49f5e7e78010"), "Mateo", "Silva", new DateOnly(1996, 11, 3), "Engineering", "[\"TypeScript\",\"API design\",\"React\"]" },
                    { Guid.Parse("63a9d4fb-6a52-4a2c-a0a6-3dfe4b5c7f12"), "Nia", "Okoye", new DateOnly(1999, 7, 19), "Data", "[\"SQL\",\"Python\"]" },
                    { Guid.Parse("b824a2f3-cb16-4a59-bc80-7c00f18c9f5d"), "Liam", "Keller", new DateOnly(1995, 2, 6), "Design", "[\"UX research\",\"Figma\"]" },
                    { Guid.Parse("cf0cf40c-90f5-4c33-8a35-9a2c85d4a3ef"), "Hana", "Sato", new DateOnly(1997, 9, 24), "Operations", "[\"Process mapping\",\"Vendor management\"]" },
                    { Guid.Parse("b2d43e2c-0e6c-42b4-b1b0-8cc663aaf5e6"), "Noah", "Ivanov", new DateOnly(1994, 12, 18), "Sales", "[\"Pipeline management\",\"Negotiation\"]" },
                    { Guid.Parse("593ea1a1-d5b2-4f59-9c4c-fb9c424f2d23"), "Priya", "Singh", new DateOnly(1998, 5, 30), "Marketing", "[\"Content strategy\",\"Campaign planning\"]" },
                    { Guid.Parse("d2151d6e-6c0a-4cd1-88de-10c525cc1fc4"), "Omar", "Zahid", new DateOnly(1993, 3, 14), "Finance", "[\"Forecasting\",\"Budgeting\"]" }
                });

            migrationBuilder.InsertData(
                table: "ApplicationSettings",
                columns: new[] { "Key", "Value" },
                values: new object[,]
                {
                    { "SkillExtraction:LlmFallbackOnly", "false" },
                    { "Groq:BaseUrl", "https://api.groq.com/openai/v1" },
                    { "Groq:Model", "llama-3.3-70b-versatile" },
                    { "Groq:TimeoutSeconds", "30" },
                    { "Groq:MaxTokens", "2048" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "EmployeeProfiles",
                keyColumn: "Id",
                keyValues: new object[]
                {
                    Guid.Parse("2b4cc3a4-90d3-4e8b-8c16-8c50f6c5f9f1"),
                    Guid.Parse("7e06c2fb-7c26-4d36-9f5a-49f5e7e78010"),
                    Guid.Parse("63a9d4fb-6a52-4a2c-a0a6-3dfe4b5c7f12"),
                    Guid.Parse("b824a2f3-cb16-4a59-bc80-7c00f18c9f5d"),
                    Guid.Parse("cf0cf40c-90f5-4c33-8a35-9a2c85d4a3ef"),
                    Guid.Parse("b2d43e2c-0e6c-42b4-b1b0-8cc663aaf5e6"),
                    Guid.Parse("593ea1a1-d5b2-4f59-9c4c-fb9c424f2d23"),
                    Guid.Parse("d2151d6e-6c0a-4cd1-88de-10c525cc1fc4")
                });

            migrationBuilder.DeleteData(
                table: "ApplicationSettings",
                keyColumn: "Key",
                keyValues: new object[]
                {
                    "SkillExtraction:LlmFallbackOnly",
                    "Groq:BaseUrl",
                    "Groq:Model",
                    "Groq:TimeoutSeconds",
                    "Groq:MaxTokens"
                });

            migrationBuilder.DropTable(
                name: "ApplicationSettings");

            migrationBuilder.DropTable(
                name: "EmployeeProfiles");
        }
    }
}
