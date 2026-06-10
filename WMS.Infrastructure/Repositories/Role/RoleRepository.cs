using Microsoft.EntityFrameworkCore;
using WMS.Domain.Interfaces;
using WMS.Infrastructure.Data;

namespace WMS.Infrastructure.Repositories.Role;

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
}