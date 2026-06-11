using Microsoft.EntityFrameworkCore;
using WMS.Domain.Interfaces;
using WMS.Infrastructure.Data;
using WMS.Domain.Entities;

namespace WMS.Infrastructure.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly WmsDbContext _context;

    public RoleRepository(
        WmsDbContext context)
    {
        _context = context;
    }

    public async Task<bool> ExistsAsync(
        int roleId)
    {
        return await _context.Roles
            .AnyAsync(r =>
                r.RoleId == roleId);
    }
    public async Task<List<Role>>
    GetAllAsync()
    {
        return await _context.Roles
            .OrderBy(r => r.RoleName)
            .ToListAsync();
    }

    public async Task<Role?>
        GetByIdAsync(
            int roleId)
    {
        return await _context.Roles
            .Include(r => r.Employees)
            .FirstOrDefaultAsync(
                r => r.RoleId == roleId);
    }

    public async Task AddAsync(
        Role role)
    {
        await _context.Roles
            .AddAsync(role);

        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(
        Role role)
    {
        _context.Roles
            .Update(role);

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(
        Role role)
    {
        _context.Roles
            .Remove(role);

        await _context.SaveChangesAsync();
    }
}