using EMS.API.Models;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using System;

namespace EMS.API.Data;
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Employee> Employees { get; set; }
    public DbSet<AppUser> AppUsers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Employee>()
            .HasIndex(e => e.Email)
            .IsUnique();
        modelBuilder.Entity<AppUser>()
            .HasIndex(u => u.Username)
            .IsUnique();

        // Seed 2 default users (BCrypt hashed passwords)
        modelBuilder.Entity<AppUser>().HasData(
            new AppUser
            {
                Id = 1,
                Username = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                Role = "Admin",
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new AppUser
            {
                Id = 2,
                Username = "viewer",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("viewer123"),
                Role = "Viewer",
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );

        modelBuilder.Entity<Employee>().HasData(
            new Employee { Id = 1, FirstName = "Priya", LastName = "Prabhu", Email = "priya.prabhu@nexacore.com", Phone = "9876543210", Department = "Engineering", Designation = "Software Engineer", Salary = 850000, JoinDate = new DateTime(2021, 3, 15, 0, 0, 0, DateTimeKind.Utc), Status = "Active", CreatedAt = new DateTime(2021, 3, 15, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2021, 3, 15, 0, 0, 0, DateTimeKind.Utc) },
            new Employee { Id = 2, FirstName = "Arjun", LastName = "Sharma", Email = "arjun.sharma@nexacore.com", Phone = "9123456780", Department = "Marketing", Designation = "Marketing Executive", Salary = 630000, JoinDate = new DateTime(2020, 7, 1, 0, 0, 0, DateTimeKind.Utc), Status = "Active", CreatedAt = new DateTime(2020, 7, 1, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2020, 7, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Employee { Id = 3, FirstName = "Neha", LastName = "Kapoor", Email = "neha.kapoor@nexacore.com", Phone = "9988776655", Department = "HR", Designation = "HR Executive", Salary = 550000, JoinDate = new DateTime(2019, 11, 23, 0, 0, 0, DateTimeKind.Utc), Status = "Active", CreatedAt = new DateTime(2019, 11, 23, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2019, 11, 23, 0, 0, 0, DateTimeKind.Utc) },
            new Employee { Id = 4, FirstName = "Rahul", LastName = "Varma", Email = "rahul.varma@nexacore.com", Phone = "9870123456", Department = "Finance", Designation = "Financial Analyst", Salary = 730000, JoinDate = new DateTime(2022, 1, 10, 0, 0, 0, DateTimeKind.Utc), Status = "Active", CreatedAt = new DateTime(2022, 1, 10, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2022, 1, 10, 0, 0, 0, DateTimeKind.Utc) },
            new Employee { Id = 5, FirstName = "Sneha", LastName = "Prasad", Email = "sneha.prasad@nexacore.com", Phone = "9765432100", Department = "Operations", Designation = "Operations Manager", Salary = 950000, JoinDate = new DateTime(2018, 6, 5, 0, 0, 0, DateTimeKind.Utc), Status = "Active", CreatedAt = new DateTime(2018, 6, 5, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2018, 6, 5, 0, 0, 0, DateTimeKind.Utc) },
            new Employee { Id = 6, FirstName = "Vikram", LastName = "Raj", Email = "vikram.raj@nexacore.com", Phone = "9654321098", Department = "Engineering", Designation = "Senior Developer", Salary = 1100000, JoinDate = new DateTime(2017, 9, 12, 0, 0, 0, DateTimeKind.Utc), Status = "Inactive", CreatedAt = new DateTime(2017, 9, 12, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2017, 9, 12, 0, 0, 0, DateTimeKind.Utc) },
            new Employee { Id = 7, FirstName = "Ananya", LastName = "Singh", Email = "ananya.singh@nexacore.com", Phone = "9543210987", Department = "Marketing", Designation = "Content Strategist", Salary = 580000, JoinDate = new DateTime(2023, 2, 28, 0, 0, 0, DateTimeKind.Utc), Status = "Inactive", CreatedAt = new DateTime(2023, 2, 28, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2023, 2, 28, 0, 0, 0, DateTimeKind.Utc) },
            new Employee { Id = 8, FirstName = "Karthik", LastName = "Rajan", Email = "karthik.rajan@nexacore.com", Phone = "9432109876", Department = "Finance", Designation = "Accounts Manager", Salary = 800000, JoinDate = new DateTime(2020, 4, 17, 0, 0, 0, DateTimeKind.Utc), Status = "Active", CreatedAt = new DateTime(2020, 4, 17, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2020, 4, 17, 0, 0, 0, DateTimeKind.Utc) },
            new Employee { Id = 9, FirstName = "Pooja", LastName = "Ghosh", Email = "pooja.ghosh@nexacore.com", Phone = "9321098765", Department = "Engineering", Designation = "DevOps Engineer", Salary = 920000, JoinDate = new DateTime(2021, 8, 30, 0, 0, 0, DateTimeKind.Utc), Status = "Active", CreatedAt = new DateTime(2021, 8, 30, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2021, 8, 30, 0, 0, 0, DateTimeKind.Utc) },
            new Employee { Id = 10, FirstName = "Amit", LastName = "Joshi", Email = "amit.joshi@nexacore.com", Phone = "9210987654", Department = "Operations", Designation = "Supply Chain Analyst", Salary = 670000, JoinDate = new DateTime(2019, 5, 20, 0, 0, 0, DateTimeKind.Utc), Status = "Active", CreatedAt = new DateTime(2019, 5, 20, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2019, 5, 20, 0, 0, 0, DateTimeKind.Utc) },
            new Employee { Id = 11, FirstName = "Lakshmi", LastName = "Chandran", Email = "lakshmi.chandran@nexacore.com", Phone = "9109876543", Department = "Marketing", Designation = "Brand Manager", Salary = 750000, JoinDate = new DateTime(2022, 11, 14, 0, 0, 0, DateTimeKind.Utc), Status = "Inactive", CreatedAt = new DateTime(2022, 11, 14, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2022, 11, 14, 0, 0, 0, DateTimeKind.Utc) },
            new Employee { Id = 12, FirstName = "Suresh", LastName = "Babu", Email = "suresh.babu@nexacore.com", Phone = "9098765432", Department = "Finance", Designation = "Tax Consultant", Salary = 870000, JoinDate = new DateTime(2016, 3, 8, 0, 0, 0, DateTimeKind.Utc), Status = "Active", CreatedAt = new DateTime(2016, 3, 8, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2016, 3, 8, 0, 0, 0, DateTimeKind.Utc) },
            new Employee { Id = 13, FirstName = "Meera", LastName = "Krishnan", Email = "meera.krishnan@nexacore.com", Phone = "9087654321", Department = "Engineering", Designation = "QA Engineer", Salary = 720000, JoinDate = new DateTime(2020, 9, 25, 0, 0, 0, DateTimeKind.Utc), Status = "Active", CreatedAt = new DateTime(2020, 9, 25, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2020, 9, 25, 0, 0, 0, DateTimeKind.Utc) },
            new Employee { Id = 14, FirstName = "Rohan", LastName = "Mehta", Email = "rohan.mehta@nexacore.com", Phone = "9876012345", Department = "HR", Designation = "HR Manager", Salary = 880000, JoinDate = new DateTime(2018, 12, 1, 0, 0, 0, DateTimeKind.Utc), Status = "Active", CreatedAt = new DateTime(2018, 12, 1, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2018, 12, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Employee { Id = 15, FirstName = "Divya", LastName = "Nair", Email = "divya.nair@nexacore.com", Phone = "9765012345", Department = "Operations", Designation = "Logistics Coordinator", Salary = 610000, JoinDate = new DateTime(2023, 6, 19, 0, 0, 0, DateTimeKind.Utc), Status = "Inactive", CreatedAt = new DateTime(2023, 6, 19, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2023, 6, 19, 0, 0, 0, DateTimeKind.Utc) }
        );
    }
}