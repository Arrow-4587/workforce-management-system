namespace WMS.Application.DTOs.Profile;

public class ProfileResponseDto
{
    public int EmployeeId { get; set; }

    public string FirstName { get; set; }
        = string.Empty;

    public string LastName { get; set; }
        = string.Empty;

    public string Email { get; set; }
        = string.Empty;

    public string PhoneNumber { get; set; }
        = string.Empty;

    public char Gender { get; set; }

    public DateTime DOB { get; set; }

    public DateTime DOJ { get; set; }

    public string Department { get; set; }
        = string.Empty;

    public string Role { get; set; }
        = string.Empty;

    public string Username { get; set; }
        = string.Empty;
}