using System.ComponentModel.DataAnnotations;

namespace WMS.Application.DTOs.Leave;

public class ApplyLeaveDto
{
    [Required]
    public string LeaveType { get; set; }
        = string.Empty;

    public string? Reason { get; set; }

    [Required]
    public DateTime FromDate { get; set; }

    [Required]
    public DateTime ToDate { get; set; }
}