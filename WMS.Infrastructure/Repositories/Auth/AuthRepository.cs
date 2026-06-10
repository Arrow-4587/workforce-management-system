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
    .Include(u => u.Employee)
    .FirstOrDefaultAsync(
        u => u.Username == username);
    }
    public async Task<bool>
    UsernameExistsAsync(string username)
    {
        return await _context.UserLogins
            .AnyAsync(u =>
                u.Username == username);
    }

    public async Task AddAsync(
        UserLogin userLogin)
    {
        await _context.UserLogins
            .AddAsync(userLogin);

        await _context.SaveChangesAsync();
    }
    public async Task<UserLogin?>
    GetByUserIdAsync(int userId)
    {
        return await _context.UserLogins
            .Include(u => u.Role)
            .Include(u => u.Employee)
            .FirstOrDefaultAsync(
                u => u.UserId == userId);
    }

    public async Task UpdateAsync(
        UserLogin userLogin)
    {
        _context.UserLogins
            .Update(userLogin);

        await _context
            .SaveChangesAsync();
    }
}