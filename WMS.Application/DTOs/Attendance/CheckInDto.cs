using System.ComponentModel.DataAnnotations;

namespace WMS.Application.DTOs.Attendance;

public class CheckInDto
{
    [Required]
    public string WorkMode { get; set; }
        = string.Empty;
}