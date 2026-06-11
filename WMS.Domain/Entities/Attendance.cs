namespace WMS.Domain.Entities;

public class Attendance
{
    public int AttendanceId { get; set; }

    public int EmpId { get; set; }

    public DateTime CheckIn { get; set; }

    public DateTime? CheckOut { get; set; }

    public double? TotalHours { get; set; }

    public string WorkMode { get; set; }
        = string.Empty;

    public DateTime AttendanceDate { get; set; }

    public Employee? Employee { get; set; }
}