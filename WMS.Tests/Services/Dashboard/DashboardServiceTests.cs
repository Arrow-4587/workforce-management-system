using Moq;
using WMS.Application.Services.Dashboard;
using WMS.Domain.Interfaces;
using Xunit;

namespace WMS.Tests.Services.Dashboard;

public class DashboardServiceTests
{
    private readonly Mock<IEmployeeRepository>
        _employeeRepositoryMock;

    private readonly Mock<IDepartmentRepository>
        _departmentRepositoryMock;

    private readonly Mock<IProjectRepository>
        _projectRepositoryMock;

    private readonly Mock<ILeaveRepository>
        _leaveRepositoryMock;

    private readonly Mock<IAttendanceRepository>
        _attendanceRepositoryMock;

    private readonly Mock<IEmployeeProjectRepository>
        _allocationRepositoryMock;

    private readonly DashboardService
        _dashboardService;

    public DashboardServiceTests()
    {
        _employeeRepositoryMock =
            new Mock<IEmployeeRepository>();

        _departmentRepositoryMock =
            new Mock<IDepartmentRepository>();

        _projectRepositoryMock =
            new Mock<IProjectRepository>();

        _leaveRepositoryMock =
            new Mock<ILeaveRepository>();

        _attendanceRepositoryMock =
            new Mock<IAttendanceRepository>();

        _allocationRepositoryMock =
            new Mock<IEmployeeProjectRepository>();

        _dashboardService =
            new DashboardService(
                _employeeRepositoryMock.Object,
                _departmentRepositoryMock.Object,
                _projectRepositoryMock.Object,
                _leaveRepositoryMock.Object,
                _attendanceRepositoryMock.Object,
                _allocationRepositoryMock.Object);
    }

    [Fact]
    public async Task GetAdminDashboardAsync_ReturnsCounts()
    {
        _employeeRepositoryMock
            .Setup(x => x.GetTotalCountAsync())
            .ReturnsAsync(100);

        _departmentRepositoryMock
            .Setup(x => x.GetTotalCountAsync())
            .ReturnsAsync(5);

        _projectRepositoryMock
            .Setup(x => x.GetTotalCountAsync())
            .ReturnsAsync(12);

        _leaveRepositoryMock
            .Setup(x => x.GetPendingCountAsync())
            .ReturnsAsync(7);

        _attendanceRepositoryMock
            .Setup(x => x.GetTodayAttendanceCountAsync())
            .ReturnsAsync(85);

        var result =
            await _dashboardService
                .GetAdminDashboardAsync();

        Assert.Equal(
            100,
            result.TotalEmployees);

        Assert.Equal(
            5,
            result.TotalDepartments);

        Assert.Equal(
            12,
            result.TotalProjects);

        Assert.Equal(
            7,
            result.PendingLeaves);

        Assert.Equal(
            85,
            result.TodayAttendance);
    }
}