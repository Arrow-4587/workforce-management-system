namespace WMS.Application.DTOs.Employee;

public class EmployeeResponseDto
{
    public int EmployeeId { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public string DepartmentName { get; set; } = string.Empty;

    public string RoleName { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;
}