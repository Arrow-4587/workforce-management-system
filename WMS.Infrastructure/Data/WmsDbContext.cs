using Microsoft.EntityFrameworkCore;
using WMS.Domain.Entities;

namespace WMS.Infrastructure.Data;

public class WmsDbContext : DbContext
{
    public WmsDbContext(DbContextOptions<WmsDbContext> options)
        : base(options)
    {
    }

    public DbSet<Role> Roles { get; set; }

    public DbSet<UserLogin> UserLogins { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Role>()
    .HasKey(r => r.RoleId);

        modelBuilder.Entity<UserLogin>()
            .HasKey(u => u.UserId);
        modelBuilder.Entity<Role>().HasData(
       new Role
       {
           RoleId = 1,
           RoleName = "Admin",
           Description = "System administrator"
       },
       new Role
       {
           RoleId = 2,
           RoleName = "Manager",
           Description = "Team manager"
       },
       new Role
       {
           RoleId = 3,
           RoleName = "Employee",
           Description = "Regular employee"
       }
       );
    }
}