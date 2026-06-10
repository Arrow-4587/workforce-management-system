namespace WMS.Domain.Entities;

public class UserLogin
{
    public int UserId { get; set; }

    public string Username { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public int RoleId { get; set; }

    public DateTime? LastLogin { get; set; }

    public Role? Role { get; set; }
}
