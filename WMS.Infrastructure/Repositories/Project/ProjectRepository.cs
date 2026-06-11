using Microsoft.EntityFrameworkCore;
using WMS.Domain.Entities;
using WMS.Domain.Interfaces;
using WMS.Infrastructure.Data;

namespace WMS.Infrastructure.Repositories;

public class ProjectRepository
    : IProjectRepository
{
    private readonly WmsDbContext _context;

    public ProjectRepository(
        WmsDbContext context)
    {
        _context = context;
    }

    public async Task<List<Project>>
        GetAllAsync()
    {
        return await _context.Projects
            .Include(p => p.Client)
            .Include(p => p.Manager)
            .OrderBy(p => p.ProjectName)
            .ToListAsync();
    }

    public async Task<Project?>
        GetByIdAsync(
            int projectId)
    {
        return await _context.Projects
            .Include(p => p.Client)
            .Include(p => p.Manager)
            .FirstOrDefaultAsync(p =>
                p.ProjectId == projectId);
    }

    public async Task AddAsync(
        Project project)
    {
        await _context.Projects
            .AddAsync(project);

        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(
        Project project)
    {
        _context.Projects
            .Update(project);

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(
        Project project)
    {
        _context.Projects
            .Remove(project);

        await _context.SaveChangesAsync();
    }

    public async Task<bool>
        ExistsAsync(
            int projectId)
    {
        return await _context.Projects
            .AnyAsync(p =>
                p.ProjectId == projectId);
    }
    public async Task<List<Project>>
    GetByManagerIdAsync(
        int managerId)
    {
        return await _context.Projects
            .Include(p => p.Client)
            .Include(p => p.Manager)
            .Where(p =>
                p.ManagerId == managerId)
            .OrderBy(p =>
                p.ProjectName)
            .ToListAsync();
    }
}