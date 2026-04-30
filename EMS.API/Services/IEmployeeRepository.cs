using EMS.API.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace EMS.API.Services;
public interface IEmployeeRepository
{
    Task<Employee> GetByIdAsync(int id);
    Task<List<Employee>> GetAllAsync();
    Task<Employee> AddAsync(Employee employee);
    Task<Employee> UpdateAsync(Employee employee);
    Task<bool> RemoveAsync(int id);
    Task<bool> EmailExistsAsync(string email, int? excludeId = null);
}