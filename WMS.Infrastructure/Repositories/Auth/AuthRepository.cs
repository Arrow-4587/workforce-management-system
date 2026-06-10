using WMS.Domain.Entities;
using WMS.Infrastructure.Data;
using WMS.Domain.Interfaces;

using Microsoft.EntityFrameworkCore;

namespace WMS.Infrastructure.Repositories.Auth;

public class AuthRepository : IAuthRepository
{
    private readonly WmsDbContext _context;

    public AuthRepository(WmsDbContext context)
    {
        _context = context;
    }

    public async Task<UserLogin?> GetByUsernameAsync(string username)
    {
        return await _context.UserLogins
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Username == username);
    }
}