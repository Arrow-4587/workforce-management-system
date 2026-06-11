namespace WMS.Application.DTOs.Dashboard;

public class AdminDashboardDto
{
    public int TotalEmployees { get; set; }

    public int TotalDepartments { get; set; }

    public int TotalProjects { get; set; }

    public int PendingLeaves { get; set; }

    public int TodayAttendance { get; set; }
}