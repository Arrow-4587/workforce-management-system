namespace WMS.Application.DTOs.Allocation;

public class AllocationResponseDto
{
    public int AllocationId { get; set; }

    public int EmployeeId { get; set; }

    public string EmployeeName { get; set; }
        = string.Empty;

    public int ProjectId { get; set; }

    public string ProjectName { get; set; }
        = string.Empty;

    public string ProjectManagerName { get; set; }
        = string.Empty;

    public string ClientName { get; set; }
        = string.Empty;

    public DateTime AllocatedOn { get; set; }

    public DateTime? ReleasedOn { get; set; }
}