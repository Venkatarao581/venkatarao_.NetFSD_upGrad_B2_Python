using EMS.API.Data;
using EMS.API.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using EMS.API.Services;
using System.Collections.Generic;
using System;

namespace EMS.API.Services;
public class EmployeeRepository : IEmployeeRepository
{
    private readonly AppDbContext _db;

    public EmployeeRepository(AppDbContext db)
    {
        _db = db;
    }

    /// <summary>Returns a single employee by ID, or null if not found.</summary>
    public async Task<Employee> GetByIdAsync(int id)
    {
        return await _db.Employees.FindAsync(id);
    }

    /// <summary>Returns all employees as a list.</summary>
    public async Task<List<Employee>> GetAllAsync()
    {
        return await _db.Employees.ToListAsync();
    }

    /// <summary>Adds a new employee and saves to database.</summary>
    public async Task<Employee> AddAsync(Employee employee)
    {
        _db.Employees.Add(employee);
        await _db.SaveChangesAsync();
        return employee;
    }

   
    public async Task<Employee> UpdateAsync(Employee employee)
    {
        employee.UpdatedAt = DateTime.UtcNow;
        _db.Employees.Update(employee);
        await _db.SaveChangesAsync();
        return employee;
    }

    public async Task<bool> RemoveAsync(int id)
    {
        var emp = await _db.Employees.FindAsync(id);
        if (emp == null) return false;
        _db.Employees.Remove(emp);
        await _db.SaveChangesAsync();
        return true;
    }
    public async Task<bool> EmailExistsAsync(string email, int? excludeId = null)
    {
        return await _db.Employees
            .AnyAsync(e => e.Email.ToLower() == email.ToLower() && e.Id != excludeId);
    }
}