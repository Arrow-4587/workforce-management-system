namespace WMS.Application.DTOs.Role;

public class CreateRoleDto
{
    public string RoleName { get; set; }
        = string.Empty;

    public string Description { get; set; }
        = string.Empty;
}