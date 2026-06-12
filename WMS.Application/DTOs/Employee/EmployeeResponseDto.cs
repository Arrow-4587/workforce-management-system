namespace WMS.Application.DTOs.Employee;

public class EmployeeResponseDto
{
    public int EmployeeId { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public char Gender { get; set; }

    public DateTime DOB { get; set; }

    public DateTime DOJ { get; set; }

    public int DepartmentId { get; set; }

    public int RoleId { get; set; }

    public string DepartmentName { get; set; } = string.Empty;

    public string RoleName { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;
}