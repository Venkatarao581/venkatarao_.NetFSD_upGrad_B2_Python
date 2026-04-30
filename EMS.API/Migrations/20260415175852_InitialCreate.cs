using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EMS.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Department = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Designation = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Salary = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    JoinDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "AppUsers",
                columns: new[] { "Id", "CreatedAt", "PasswordHash", "Role", "Username" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "$2a$11$.D1YvAgfNMpADhpy8WwSdu.1avdqtUv1p/nGrA/bn2od6K/yIQN2.", "Admin", "admin" },
                    { 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "$2a$11$y8PTZhcRkmKq1m1iL/AgjOKBD9DTnUNLbFnXcJLwYxfY0UECQh9TG", "Viewer", "viewer" }
                });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "CreatedAt", "Department", "Designation", "Email", "FirstName", "JoinDate", "LastName", "Phone", "Salary", "Status", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2021, 3, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Engineering", "Software Engineer", "priya.prabhu@nexacore.com", "Priya", new DateTime(2021, 3, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Prabhu", "9876543210", 850000m, "Active", new DateTime(2021, 3, 15, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, new DateTime(2020, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Marketing", "Marketing Executive", "arjun.sharma@nexacore.com", "Arjun", new DateTime(2020, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Sharma", "9123456780", 630000m, "Active", new DateTime(2020, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3, new DateTime(2019, 11, 23, 0, 0, 0, 0, DateTimeKind.Utc), "HR", "HR Executive", "neha.kapoor@nexacore.com", "Neha", new DateTime(2019, 11, 23, 0, 0, 0, 0, DateTimeKind.Utc), "Kapoor", "9988776655", 550000m, "Active", new DateTime(2019, 11, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4, new DateTime(2022, 1, 10, 0, 0, 0, 0, DateTimeKind.Utc), "Finance", "Financial Analyst", "rahul.varma@nexacore.com", "Rahul", new DateTime(2022, 1, 10, 0, 0, 0, 0, DateTimeKind.Utc), "Varma", "9870123456", 730000m, "Active", new DateTime(2022, 1, 10, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 5, new DateTime(2018, 6, 5, 0, 0, 0, 0, DateTimeKind.Utc), "Operations", "Operations Manager", "sneha.prasad@nexacore.com", "Sneha", new DateTime(2018, 6, 5, 0, 0, 0, 0, DateTimeKind.Utc), "Prasad", "9765432100", 950000m, "Active", new DateTime(2018, 6, 5, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 6, new DateTime(2017, 9, 12, 0, 0, 0, 0, DateTimeKind.Utc), "Engineering", "Senior Developer", "vikram.raj@nexacore.com", "Vikram", new DateTime(2017, 9, 12, 0, 0, 0, 0, DateTimeKind.Utc), "Raj", "9654321098", 1100000m, "Inactive", new DateTime(2017, 9, 12, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 7, new DateTime(2023, 2, 28, 0, 0, 0, 0, DateTimeKind.Utc), "Marketing", "Content Strategist", "ananya.singh@nexacore.com", "Ananya", new DateTime(2023, 2, 28, 0, 0, 0, 0, DateTimeKind.Utc), "Singh", "9543210987", 580000m, "Inactive", new DateTime(2023, 2, 28, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 8, new DateTime(2020, 4, 17, 0, 0, 0, 0, DateTimeKind.Utc), "Finance", "Accounts Manager", "karthik.rajan@nexacore.com", "Karthik", new DateTime(2020, 4, 17, 0, 0, 0, 0, DateTimeKind.Utc), "Rajan", "9432109876", 800000m, "Active", new DateTime(2020, 4, 17, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 9, new DateTime(2021, 8, 30, 0, 0, 0, 0, DateTimeKind.Utc), "Engineering", "DevOps Engineer", "pooja.ghosh@nexacore.com", "Pooja", new DateTime(2021, 8, 30, 0, 0, 0, 0, DateTimeKind.Utc), "Ghosh", "9321098765", 920000m, "Active", new DateTime(2021, 8, 30, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 10, new DateTime(2019, 5, 20, 0, 0, 0, 0, DateTimeKind.Utc), "Operations", "Supply Chain Analyst", "amit.joshi@nexacore.com", "Amit", new DateTime(2019, 5, 20, 0, 0, 0, 0, DateTimeKind.Utc), "Joshi", "9210987654", 670000m, "Active", new DateTime(2019, 5, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 11, new DateTime(2022, 11, 14, 0, 0, 0, 0, DateTimeKind.Utc), "Marketing", "Brand Manager", "lakshmi.chandran@nexacore.com", "Lakshmi", new DateTime(2022, 11, 14, 0, 0, 0, 0, DateTimeKind.Utc), "Chandran", "9109876543", 750000m, "Inactive", new DateTime(2022, 11, 14, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 12, new DateTime(2016, 3, 8, 0, 0, 0, 0, DateTimeKind.Utc), "Finance", "Tax Consultant", "suresh.babu@nexacore.com", "Suresh", new DateTime(2016, 3, 8, 0, 0, 0, 0, DateTimeKind.Utc), "Babu", "9098765432", 870000m, "Active", new DateTime(2016, 3, 8, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 13, new DateTime(2020, 9, 25, 0, 0, 0, 0, DateTimeKind.Utc), "Engineering", "QA Engineer", "meera.krishnan@nexacore.com", "Meera", new DateTime(2020, 9, 25, 0, 0, 0, 0, DateTimeKind.Utc), "Krishnan", "9087654321", 720000m, "Active", new DateTime(2020, 9, 25, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 14, new DateTime(2018, 12, 1, 0, 0, 0, 0, DateTimeKind.Utc), "HR", "HR Manager", "rohan.mehta@nexacore.com", "Rohan", new DateTime(2018, 12, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Mehta", "9876012345", 880000m, "Active", new DateTime(2018, 12, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 15, new DateTime(2023, 6, 19, 0, 0, 0, 0, DateTimeKind.Utc), "Operations", "Logistics Coordinator", "divya.nair@nexacore.com", "Divya", new DateTime(2023, 6, 19, 0, 0, 0, 0, DateTimeKind.Utc), "Nair", "9765012345", 610000m, "Inactive", new DateTime(2023, 6, 19, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppUsers_Username",
                table: "AppUsers",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_Email",
                table: "Employees",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppUsers");

            migrationBuilder.DropTable(
                name: "Employees");
        }
    }
}
