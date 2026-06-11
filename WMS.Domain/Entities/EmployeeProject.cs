namespace WMS.Domain.Entities;

public class EmployeeProject
{
    public int AllocationId { get; set; }

    public int EmployeeId { get; set; }

    public int ProjectId { get; set; }

    public DateTime AllocatedOn { get; set; }

    public DateTime? ReleasedOn { get; set; }

    public Employee? Employee { get; set; }

    public Project? Project { get; set; }
}