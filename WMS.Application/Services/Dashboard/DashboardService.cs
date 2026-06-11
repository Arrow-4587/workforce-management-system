using WMS.Application.DTOs.Dashboard;
using WMS.Domain.Interfaces;

namespace WMS.Application.Services.Dashboard;

public class DashboardService
    : IDashboardService
{
    private readonly IEmployeeRepository
        _employeeRepository;

    private readonly IDepartmentRepository
        _departmentRepository;

    private readonly IProjectRepository
        _projectRepository;

    private readonly ILeaveRepository
        _leaveRepository;

    private readonly IAttendanceRepository
        _attendanceRepository;

    private readonly IEmployeeProjectRepository
        _allocationRepository;

    public DashboardService(
        IEmployeeRepository employeeRepository,
        IDepartmentRepository departmentRepository,
        IProjectRepository projectRepository,
        ILeaveRepository leaveRepository,
        IAttendanceRepository attendanceRepository,
        IEmployeeProjectRepository allocationRepository)
    {
        _employeeRepository =
            employeeRepository;

        _departmentRepository =
            departmentRepository;

        _projectRepository =
            projectRepository;

        _leaveRepository =
            leaveRepository;

        _attendanceRepository =
            attendanceRepository;

        _allocationRepository =
            allocationRepository;
    }

    public async Task<AdminDashboardDto>
        GetAdminDashboardAsync()
    {
        return new AdminDashboardDto
        {
            TotalEmployees =
                await _employeeRepository
                    .GetTotalCountAsync(),

            TotalDepartments =
                await _departmentRepository
                    .GetTotalCountAsync(),

            TotalProjects =
                await _projectRepository
                    .GetTotalCountAsync(),

            PendingLeaves =
                await _leaveRepository
                    .GetPendingCountAsync(),

            TodayAttendance =
                await _attendanceRepository
                    .GetTodayAttendanceCountAsync()
        };
    }

    public async Task<ManagerDashboardDto>
        GetManagerDashboardAsync(
            int managerId)
    {
        var projects =
            await _projectRepository
                .GetByManagerIdAsync(
                    managerId);

        int employeeCount = 0;

        foreach (var project in projects)
        {
            var employees =
                await _allocationRepository
                    .GetByProjectIdAsync(
                        project.ProjectId);

            employeeCount +=
                employees.Count;
        }

        return new ManagerDashboardDto
        {
            MyProjects =
                projects.Count,

            AllocatedEmployees =
                employeeCount,

            PendingLeaves =
                await _leaveRepository
                    .GetPendingCountAsync()
        };
    }

    public async Task<EmployeeDashboardDto>
        GetEmployeeDashboardAsync(
            int employeeId)
    {
        var projects =
            await _allocationRepository
                .GetByEmployeeIdAsync(
                    employeeId);

        return new EmployeeDashboardDto
        {
            MyProjects =
                projects.Count,

            MyPendingLeaves =
                (await _leaveRepository
                    .GetByEmployeeIdAsync(
                        employeeId))
                .Count(l =>
                    l.Status == "Pending"),

            AttendanceThisMonth =
                await _attendanceRepository
                    .GetMonthlyAttendanceCountAsync(
                        employeeId,
                        DateTime.Today.Year,
                        DateTime.Today.Month)
        };
    }
}