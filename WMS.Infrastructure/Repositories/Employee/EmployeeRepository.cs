using Microsoft.EntityFrameworkCore;
using WMS.Domain.Entities;
using WMS.Domain.Interfaces;
using WMS.Infrastructure.Data;
using WMS.Infrastructure.Repositories;

namespace WMS.Infrastructure.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly WmsDbContext _context;

    public EmployeeRepository(WmsDbContext context)
    {
        _context = context;
    }
    public async Task<List<Employee>> GetAllAsync()
    {
        return await _context.Employees
            .Include(e => e.Department)
            .Include(e => e.Role)
            .AsNoTracking()
            .ToListAsync();
    }
    public async Task<Employee?> GetByIdAsync(int employeeId)
    {
        return await _context.Employees
            .Include(e => e.Department)
            .Include(e => e.Role)
            .FirstOrDefaultAsync(e => e.EmployeeId == employeeId);
    }
    public async Task AddAsync(Employee employee)
    {
        await _context.Employees.AddAsync(employee);

        await _context.SaveChangesAsync();
    }
    public async Task UpdateAsync(Employee employee)
    {
        _context.Employees.Update(employee);

        await _context.SaveChangesAsync();
    }
    public async Task DeleteAsync(Employee employee)
    {
        _context.Employees.Remove(employee);

        await _context.SaveChangesAsync();
    }
    public async Task<bool> ExistsAsync(int employeeId)
    {
        return await _context.Employees
            .AnyAsync(e => e.EmployeeId == employeeId);
    }
    public async Task<Employee?> GetByEmailAsync(string email)
    {
        return await _context.Employees
            .FirstOrDefaultAsync(e => e.Email == email);
    }
    public async Task<List<Employee>> SearchByNameAsync(string name)
    {
        return await _context.Employees
            .Include(e => e.Department)
            .Include(e => e.Role)
            .Where(e =>
                e.FirstName.Contains(name) ||
                e.LastName.Contains(name))
            .AsNoTracking()
            .ToListAsync();
    }
    public async Task<List<Employee>> GetByDepartmentAsync(int departmentId)
    {
        return await _context.Employees
            .Include(e => e.Department)
            .Include(e => e.Role)
            .Where(e => e.DepartmentId == departmentId)
            .AsNoTracking()
            .ToListAsync();
    }
    public async Task<List<Employee>> GetByRoleAsync(int roleId)
    {
        return await _context.Employees
            .Include(e => e.Department)
            .Include(e => e.Role)
            .Where(e => e.RoleId == roleId)
            .AsNoTracking()
            .ToListAsync();
    }
    public async Task<bool> EmailExistsAsync(
    string email,
    int employeeId)
    {
        return await _context.Employees
            .AnyAsync(e =>
                e.Email == email &&
                e.EmployeeId != employeeId);
    }
    public async Task<bool>
    DepartmentHasEmployeesAsync(
        int departmentId)
    {
        return await _context.Employees
            .AnyAsync(e =>
                e.DepartmentId == departmentId);
    }
}