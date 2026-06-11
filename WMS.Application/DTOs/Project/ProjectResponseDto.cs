namespace WMS.Application.DTOs.Project;

public class ProjectResponseDto
{
    public int ProjectId { get; set; }

    public string ProjectName { get; set; }
        = string.Empty;

    public int ClientId { get; set; }

    public string ClientName { get; set; }
        = string.Empty;

    public int ManagerId { get; set; }

    public string ManagerName { get; set; }
        = string.Empty;

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public string Status { get; set; }
        = string.Empty;
}