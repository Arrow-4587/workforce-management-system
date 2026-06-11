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
    public DbSet<Department> Departments { get; set; }

    public DbSet<Employee> Employees { get; set; }
    public DbSet<Attendance> Attendances
    {
        get;
        set;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
//--------------------------------------------------------------------//
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
//-------------------------------------------------------------------//
        modelBuilder.Entity<Department>().HasData(
    new Department
    {
        DepartmentId = 1,
        DepartmentName = "IT",
        Description = "Information Technology",
        CreatedOn = new DateTime(2026, 1, 1)
    },
    new Department
    {
        DepartmentId = 2,
        DepartmentName = "HR",
        Description = "Human Resources",
        CreatedOn = new DateTime(2026, 1, 1)
    },
    new Department
    {
        DepartmentId = 3,
        DepartmentName = "Finance",
        Description = "Finance Department",
        CreatedOn = new DateTime(2026, 1, 1)
    }
);
        //-------------------------------------------------------------------//
        modelBuilder.Entity<UserLogin>()
            .HasOne(u => u.Employee)
            .WithOne(e => e.UserLogin)
            .HasForeignKey<UserLogin>(
                u => u.EmployeeId);

        //-------------------------------------------------------------------//
        modelBuilder.Entity<Attendance>()
    .HasOne(a => a.Employee)
    .WithMany(e => e.Attendances)
    .HasForeignKey(a => a.EmpId);
    }

}