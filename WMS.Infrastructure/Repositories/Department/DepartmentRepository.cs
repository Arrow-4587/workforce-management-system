using Microsoft.EntityFrameworkCore;
using WMS.Domain.Interfaces;
using WMS.Infrastructure.Data;
using DepartmentEntity = WMS.Domain.Entities.Department;

namespace WMS.Infrastructure.Repositories.Department;

public class DepartmentRepository : IDepartmentRepository
{
    private readonly WmsDbContext _context;

    public DepartmentRepository(WmsDbContext context)
    {
        _context = context;
    }

    public async Task<List<DepartmentEntity>> GetAllAsync()
    {
        return await _context.Departments
            .OrderBy(d => d.DepartmentName)
            .ToListAsync();
    }

    public async Task<DepartmentEntity?> GetByIdAsync(
        int departmentId)
    {
        return await _context.Departments
            .FirstOrDefaultAsync(d =>
                d.DepartmentId == departmentId);
    }

    public async Task<DepartmentEntity?> GetByNameAsync(
        string departmentName)
    {
        return await _context.Departments
            .FirstOrDefaultAsync(d =>
                d.DepartmentName == departmentName);
    }

    public async Task AddAsync(
        DepartmentEntity department)
    {
        await _context.Departments.AddAsync(department);

        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(
        DepartmentEntity department)
    {
        _context.Departments.Update(department);

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(
        DepartmentEntity department)
    {
        _context.Departments.Remove(department);

        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(
        int departmentId)
    {
        return await _context.Departments
            .AnyAsync(d =>
                d.DepartmentId == departmentId);
    }
    public async Task<List<DepartmentEntity>>
    SearchByNameAsync(string name)
    {
        return await _context.Departments
            .Where(d =>
                d.DepartmentName.Contains(name))
            .OrderBy(d =>
                d.DepartmentName)
            .ToListAsync();
    }
    public async Task<int>
    GetTotalCountAsync()
{
    return await _context.Departments
        .CountAsync();
}
}