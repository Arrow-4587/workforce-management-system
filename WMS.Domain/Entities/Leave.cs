namespace WMS.Domain.Entities;

public class Leave
{
    public int LeaveId { get; set; }

    public int EmpId { get; set; }

    public string LeaveType { get; set; }
        = string.Empty;

    public string? Reason { get; set; }

    public DateTime FromDate { get; set; }

    public DateTime ToDate { get; set; }

    public string Status { get; set; }
        = "Pending";

    public DateTime AppliedOn { get; set; }

    public int? ApprovedBy { get; set; }

    public DateTime? ApprovedOn { get; set; }

    public Employee? Employee { get; set; }
}