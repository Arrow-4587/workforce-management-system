namespace WMS.Domain.Entities;

public class UserLogin
{
    public int UserId { get; set; }

    public int? EmployeeId { get; set; }

    public string Username { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public int RoleId { get; set; }

    public DateTime? LastLogin { get; set; }
    public bool IsFirstLogin { get; set; } = true;
    

    public Role? Role { get; set; }

    public Employee? Employee { get; set; }
}