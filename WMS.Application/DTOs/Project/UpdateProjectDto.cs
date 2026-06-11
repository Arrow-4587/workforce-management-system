using System.ComponentModel.DataAnnotations;

namespace WMS.Application.DTOs.Project;

public class UpdateProjectDto
{
    [Required]
    public string ProjectName { get; set; }
        = string.Empty;

    [Required]
    public int ClientId { get; set; }

    [Required]
    public int ManagerId { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public string Status { get; set; }
        = "Active";
}