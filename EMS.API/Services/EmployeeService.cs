using EMS.API.Data;
using EMS.API.DTOs;
using EMS.API.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace EMS.API.Services;
public class EmployeeService
{
    private readonly IEmployeeRepository _repo;
    private readonly AppDbContext _db;

    public EmployeeService(IEmployeeRepository repo, AppDbContext db)
    {
        _repo = repo;
        _db = db;
    }
    public async Task<PagedResult<EmployeeResponseDto>> GetAllAsync(EmployeeQueryParams q)
    {
        var query = _db.Employees.AsQueryable();

        // Search — case-insensitive LIKE on firstName+lastName and email
        if (!string.IsNullOrWhiteSpace(q.Search))
        {
            var term = q.Search.ToLower();
            query = query.Where(e =>
                (e.FirstName.ToLower() + " " + e.LastName.ToLower()).Contains(term) ||
                e.Email.ToLower().Contains(term));
        }

        // Department filter — exact match
        if (!string.IsNullOrWhiteSpace(q.Department))
            query = query.Where(e => e.Department == q.Department);

        // Status filter — exact match
        if (!string.IsNullOrWhiteSpace(q.Status))
            query = query.Where(e => e.Status == q.Status);

        // Sorting — ORDER BY in SQL
        query = q.SortBy?.ToLower() switch
        {
            "salary" => q.SortDir == "desc"
                ? query.OrderByDescending(e => e.Salary)
                : query.OrderBy(e => e.Salary),
            "joindate" => q.SortDir == "desc"
                ? query.OrderByDescending(e => e.JoinDate)
                : query.OrderBy(e => e.JoinDate),
            _ => q.SortDir == "desc"
                ? query.OrderByDescending(e => e.LastName).ThenByDescending(e => e.FirstName)
                : query.OrderBy(e => e.LastName).ThenBy(e => e.FirstName),
        };

        // Total count before paging
        var totalCount = await query.CountAsync();

        // Pagination — capped at 100 per page
        var pageSize = Math.Min(q.PageSize, 100);
        var page = Math.Max(q.Page, 1);
        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

        var employees = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<EmployeeResponseDto>
        {
            Data = employees.Select(MapToDto).ToList(),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            TotalPages = totalPages,
            HasNextPage = page < totalPages,
            HasPrevPage = page > 1
        };
    }
    public async Task<EmployeeResponseDto> GetByIdAsync(int id)
    {
        var emp = await _repo.GetByIdAsync(id);
        return emp == null ? null : MapToDto(emp);
    }
    public async Task<EmployeeResponseDto> AddAsync(EmployeeRequestDto dto)
    {
        if (await _repo.EmailExistsAsync(dto.Email))
            return null; // caller converts to 409 Conflict

        var emp = MapFromDto(dto);
        var created = await _repo.AddAsync(emp);
        return MapToDto(created);
    }
    public async Task<(EmployeeResponseDto Result, bool EmailConflict)> UpdateAsync(int id, EmployeeRequestDto dto)
    {
        var emp = await _repo.GetByIdAsync(id);
        if (emp == null) return (null, false);

        if (await _repo.EmailExistsAsync(dto.Email, excludeId: id))
            return (null, true);

        emp.FirstName = dto.FirstName;
        emp.LastName = dto.LastName;
        emp.Email = dto.Email;
        emp.Phone = dto.Phone;
        emp.Department = dto.Department;
        emp.Designation = dto.Designation;
        emp.Salary = dto.Salary;
        emp.JoinDate = dto.JoinDate;
        emp.Status = dto.Status;

        var updated = await _repo.UpdateAsync(emp);
        return (MapToDto(updated), false);
    }
    public async Task<bool> DeleteAsync(int id)
    {
        return await _repo.RemoveAsync(id);
    }
    public async Task<DashboardSummaryDto> GetDashboardAsync()
    {
        var total = await _db.Employees.CountAsync();
        var active = await _db.Employees.CountAsync(e => e.Status == "Active");
        var inactive = await _db.Employees.CountAsync(e => e.Status == "Inactive");
        var deptCount = await _db.Employees.Select(e => e.Department).Distinct().CountAsync();

        var breakdown = await _db.Employees
            .GroupBy(e => e.Department)
            .Select(g => new DepartmentCountDto
            {
                Department = g.Key,
                Count = g.Count(),
                Percentage = total > 0 ? Math.Round((double)g.Count() / total * 100, 1) : 0
            })
            .OrderBy(d => d.Department)
            .ToListAsync();

        var recent = await _db.Employees
            .OrderByDescending(e => e.CreatedAt)
            .ThenByDescending(e => e.Id)
            .Take(5)
            .ToListAsync();

        return new DashboardSummaryDto
        {
            Total = total,
            Active = active,
            Inactive = inactive,
            Departments = deptCount,
            DepartmentBreakdown = breakdown,
            RecentEmployees = recent.Select(MapToDto).ToList()
        };
    }
    public static EmployeeResponseDto MapToDto(Employee e) => new()
    {
        Id = e.Id,
        FirstName = e.FirstName,
        LastName = e.LastName,
        Email = e.Email,
        Phone = e.Phone,
        Department = e.Department,
        Designation = e.Designation,
        Salary = e.Salary,
        JoinDate = e.JoinDate,
        Status = e.Status,
        CreatedAt = e.CreatedAt
    };

    private static Employee MapFromDto(EmployeeRequestDto dto) => new()
    {
        FirstName = dto.FirstName,
        LastName = dto.LastName,
        Email = dto.Email,
        Phone = dto.Phone,
        Department = dto.Department,
        Designation = dto.Designation,
        Salary = dto.Salary,
        JoinDate = dto.JoinDate,
        Status = dto.Status,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };
}