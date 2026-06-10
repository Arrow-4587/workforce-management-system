using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using WMS.Domain.Entities;

namespace WMS.Infrastructure.Data;

public static class DbSeeder
{
    public static async Task SeedAdminAsync(WmsDbContext context)
    {
        if (await context.UserLogins.AnyAsync())
            return;

        var adminRole = await context.Roles
            .FirstOrDefaultAsync(r => r.RoleName == "Admin");

        if (adminRole == null)
            return;

        var adminUser = new UserLogin
        {
            Username = "admin",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
            RoleId = adminRole.RoleId
        };

        context.UserLogins.Add(adminUser);

        await context.SaveChangesAsync();
    }
}