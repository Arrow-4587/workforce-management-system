using System.ComponentModel.DataAnnotations;

namespace WMS.Application.DTOs.Allocation;

public class AllocateEmployeeDto
{
    [Required]
    public int EmployeeId { get; set; }

    [Required]
    public int ProjectId { get; set; }
}