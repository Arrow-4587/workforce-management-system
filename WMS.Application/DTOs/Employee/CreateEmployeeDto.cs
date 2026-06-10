using System.ComponentModel.DataAnnotations;

namespace WMS.Application.DTOs.Employee;

public class CreateEmployeeDto
{
    [Required]
    [StringLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(80)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [RegularExpression(@"^[0-9]{10,15}$",
      ErrorMessage = "Phone number must contain 10-15 digits.")]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required]
    public char Gender { get; set; }

    [Required]
    public DateTime DOB { get; set; }

    [Required]
    public DateTime DOJ { get; set; }

    [Required]
    public int DepartmentId { get; set; }

    [Required]
    public int RoleId { get; set; }
}