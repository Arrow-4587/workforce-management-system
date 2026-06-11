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
    public DbSet<Leave> Leaves
    {
        get;
        set;
    }
    public DbSet<Client> Clients
    {
        get;
        set;
    }
    public DbSet<Project> Projects
    {
        get;
        set;
    }
    public DbSet<EmployeeProject>
    EmployeeProjects
    {
        get;
        set;
    }
    public DbSet<Announcement>
Announcements
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

        //--------------------------------------------------------------------//
        modelBuilder.Entity<Leave>()
        .HasOne(l => l.Employee)
        .WithMany(e => e.Leaves)
        .HasForeignKey(l => l.EmpId);
        //----------------------------------------------------------------//
        modelBuilder.Entity<Project>()
    .HasOne(p => p.Client)
    .WithMany(c => c.Projects)
    .HasForeignKey(p => p.ClientId);
        //--------------------------------//
        modelBuilder.Entity<Project>()
    .HasOne(p => p.Manager)
    .WithMany()
    .HasForeignKey(p => p.ManagerId)
    .OnDelete(DeleteBehavior.Restrict);

        //------------------------------------------------------------//

        modelBuilder.Entity<EmployeeProject>()
    .HasKey(ep => ep.AllocationId);

        //---------------------------------------------------------//
        modelBuilder.Entity<EmployeeProject>()
        .HasOne(ep => ep.Employee)
        .WithMany(e => e.EmployeeProjects)
        .HasForeignKey(ep => ep.EmployeeId);
        //------------------------------------------------------------//
        modelBuilder.Entity<EmployeeProject>()
    .HasOne(ep => ep.Project)
    .WithMany(p => p.EmployeeProjects)
    .HasForeignKey(ep => ep.ProjectId);
        //------------------------------------------------------------//
        modelBuilder.Entity<Announcement>()
    .HasOne(a => a.Creator)
    .WithMany()
    .HasForeignKey(a => a.CreatedBy)
    .OnDelete(DeleteBehavior.Restrict);
    }
}