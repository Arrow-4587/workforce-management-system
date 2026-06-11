namespace WMS.Application.DTOs.Attendance;

public class AttendanceResponseDto
{
    public int AttendanceId { get; set; }

    public DateTime CheckIn { get; set; }

    public DateTime? CheckOut { get; set; }

    public double? TotalHours { get; set; }

    public string WorkMode { get; set; }
        = string.Empty;

    public DateTime AttendanceDate { get; set; }
}