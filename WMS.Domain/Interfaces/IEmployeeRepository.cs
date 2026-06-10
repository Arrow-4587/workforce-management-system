using WMS.Domain.Entities;

namespace WMS.Domain.Interfaces;

public interface IEmployeeRepository
{
    Task<List<Employee>> GetAllAsync();

    Task<Employee?> GetByIdAsync(int employeeId);

    Task AddAsync(Employee employee);

    Task UpdateAsync(Employee employee);

    Task DeleteAsync(Employee employee);

    Task<bool> ExistsAsync(int employeeId);
    Task<Employee?> GetByEmailAsync(string email);

    Task<List<Employee>> SearchByNameAsync(string name);

    Task<List<Employee>> GetByDepartmentAsync(int departmentId);

    Task<List<Employee>> GetByRoleAsync(int roleId);
    Task<bool> EmailExistsAsync(string email, int employeeId);
    Task<bool> DepartmentHasEmployeesAsync(
    int departmentId);

    Task<Employee?> GetByUserIdAsync(
    int userId);
}