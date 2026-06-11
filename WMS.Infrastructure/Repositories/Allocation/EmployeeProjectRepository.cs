using Microsoft.EntityFrameworkCore;
using WMS.Domain.Entities;
using WMS.Domain.Interfaces;
using WMS.Infrastructure.Data;

namespace WMS.Infrastructure.Repositories;

public class EmployeeProjectRepository
    : IEmployeeProjectRepository
{
    private readonly WmsDbContext _context;

    public EmployeeProjectRepository(
        WmsDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(
        EmployeeProject allocation)
    {
        await _context.EmployeeProjects
            .AddAsync(allocation);

        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(
        EmployeeProject allocation)
    {
        _context.EmployeeProjects
            .Update(allocation);

        await _context.SaveChangesAsync();
    }

    public async Task<EmployeeProject?>
        GetByIdAsync(
            int allocationId)
    {
        return await _context.EmployeeProjects
            .Include(ep => ep.Employee)
            .Include(ep => ep.Project)
            .FirstOrDefaultAsync(ep =>
                ep.AllocationId == allocationId);
    }

    public async Task<List<EmployeeProject>>
        GetByProjectIdAsync(
            int projectId)
    {
        return await _context.EmployeeProjects
            .Include(ep => ep.Employee)
            .Where(ep =>
                ep.ProjectId == projectId &&
                ep.ReleasedOn == null)
            .ToListAsync();
    }

    public async Task<List<EmployeeProject>>
      GetByEmployeeIdAsync(
          int employeeId)
    {
        return await _context.EmployeeProjects
            .Include(ep => ep.Project)
            .Where(ep =>
                ep.EmployeeId == employeeId &&
                ep.ReleasedOn == null)
            .ToListAsync();
    }
    public async Task<EmployeeProject?>
    GetActiveAllocationAsync(
        int employeeId,
        int projectId)
    {
        return await _context.EmployeeProjects
            .FirstOrDefaultAsync(ep =>
                ep.EmployeeId == employeeId &&
                ep.ProjectId == projectId &&
                ep.ReleasedOn == null);
    }
    public async Task<bool>
    IsEmployeeUnderManagerAsync(
        int employeeId,
        int managerId)
    {
        return await _context.EmployeeProjects
            .Include(ep => ep.Project)
            .AnyAsync(ep =>
                ep.EmployeeId == employeeId &&
                ep.ReleasedOn == null &&
                ep.Project != null &&
                ep.Project.ManagerId == managerId);
    }
    public async Task<List<int>>
    GetEmployeeIdsByManagerAsync(
        int managerId)
{
    return await _context.EmployeeProjects
        .Include(ep => ep.Project)
        .Where(ep =>
            ep.ReleasedOn == null &&
            ep.Project != null &&
            ep.Project.ManagerId == managerId)
        .Select(ep =>
            ep.EmployeeId)
        .Distinct()
        .ToListAsync();
}
}