using Microsoft.EntityFrameworkCore;
using WMS.Domain.Interfaces;
using WMS.Infrastructure.Data;

namespace WMS.Infrastructure.Repositories.Department;

public class DepartmentRepository
    : IDepartmentRepository
{
    private readonly WmsDbContext _context;

    public DepartmentRepository(
        WmsDbContext context)
    {
        _context = context;
    }

    public async Task<bool> ExistsAsync(
        int departmentId)
    {
        return await _context.Departments
            .AnyAsync(d =>
                d.DepartmentId == departmentId);
    }
}