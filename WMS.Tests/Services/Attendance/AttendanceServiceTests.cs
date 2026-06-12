using Moq;
using WMS.Application.DTOs.Attendance;
using WMS.Application.Services.Attendance;
using WMS.Domain.Interfaces;
using Xunit;

using AttendanceEntity =
    WMS.Domain.Entities.Attendance;

namespace WMS.Tests.Services.Attendance;

public class AttendanceServiceTests
{
    private readonly Mock<IAttendanceRepository>
        _attendanceRepositoryMock;

    private readonly Mock<IAuditLogRepository>
        _auditLogRepositoryMock;

    private readonly AttendanceService
        _attendanceService;

    public AttendanceServiceTests()
    {
        _attendanceRepositoryMock =
            new Mock<IAttendanceRepository>();

        _auditLogRepositoryMock =
            new Mock<IAuditLogRepository>();

        _attendanceService =
            new AttendanceService(
                _attendanceRepositoryMock.Object,
                _auditLogRepositoryMock.Object);
    }

    [Fact]
    public async Task CheckInAsync_ValidRequest_ReturnsAttendance()
    {
        _attendanceRepositoryMock
            .Setup(x =>
                x.GetTodayAttendanceAsync(1))
            .ReturnsAsync(
                (AttendanceEntity?)null);

        var dto = new CheckInDto
        {
            WorkMode = "Remote"
        };

        var result =
            await _attendanceService
                .CheckInAsync(
                    1,
                    dto);

        Assert.NotNull(result);

        Assert.Equal(
            "Remote",
            result.WorkMode);

        _attendanceRepositoryMock.Verify(
            x => x.AddAsync(
                It.IsAny<AttendanceEntity>()),
            Times.Once);

        _auditLogRepositoryMock.Verify(
            x => x.AddAsync(
                It.IsAny<WMS.Domain.Entities.AuditLog>()),
            Times.Once);
    }

    [Fact]
    public async Task CheckInAsync_AlreadyCheckedIn_ThrowsException()
    {
        _attendanceRepositoryMock
            .Setup(x =>
                x.GetTodayAttendanceAsync(1))
            .ReturnsAsync(
                new AttendanceEntity());

        var dto = new CheckInDto
        {
            WorkMode = "Remote"
        };

        var ex =
            await Assert.ThrowsAsync<Exception>(
                () =>
                    _attendanceService
                        .CheckInAsync(
                            1,
                            dto));

        Assert.Equal(
            "Already checked in today.",
            ex.Message);
    }

    [Fact]
    public async Task CheckInAsync_InvalidWorkMode_ThrowsException()
    {
        _attendanceRepositoryMock
            .Setup(x =>
                x.GetTodayAttendanceAsync(1))
            .ReturnsAsync(
                (AttendanceEntity?)null);

        var dto = new CheckInDto
        {
            WorkMode = "Invalid"
        };

        var ex =
            await Assert.ThrowsAsync<Exception>(
                () =>
                    _attendanceService
                        .CheckInAsync(
                            1,
                            dto));

        Assert.Equal(
            "WorkMode must be Remote, Office or Hybrid.",
            ex.Message);
    }

    [Fact]
    public async Task CheckOutAsync_CheckInNotFound_ThrowsException()
    {
        _attendanceRepositoryMock
            .Setup(x =>
                x.GetTodayAttendanceAsync(1))
            .ReturnsAsync(
                (AttendanceEntity?)null);

        var ex =
            await Assert.ThrowsAsync<Exception>(
                () =>
                    _attendanceService
                        .CheckOutAsync(
                            1));

        Assert.Equal(
            "Check-in not found.",
            ex.Message);
    }

    [Fact]
    public async Task CheckOutAsync_AlreadyCheckedOut_ThrowsException()
    {
        var attendance =
            new AttendanceEntity
            {
                CheckIn =
                    DateTime.UtcNow
                        .AddHours(-8),

                CheckOut =
                    DateTime.UtcNow
            };

        _attendanceRepositoryMock
            .Setup(x =>
                x.GetTodayAttendanceAsync(1))
            .ReturnsAsync(
                attendance);

        var ex =
            await Assert.ThrowsAsync<Exception>(
                () =>
                    _attendanceService
                        .CheckOutAsync(
                            1));

        Assert.Equal(
            "Already checked out.",
            ex.Message);
    }

    [Fact]
    public async Task CheckOutAsync_ValidRequest_UpdatesAttendance()
    {
        var attendance =
            new AttendanceEntity
            {
                AttendanceId = 1,
                EmpId = 1,
                CheckIn =
                    DateTime.UtcNow
                        .AddHours(-8),
                CheckOut = null
            };

        _attendanceRepositoryMock
            .Setup(x =>
                x.GetTodayAttendanceAsync(1))
            .ReturnsAsync(
                attendance);

        var result =
            await _attendanceService
                .CheckOutAsync(
                    1);

        Assert.NotNull(
            result.CheckOut);

        Assert.True(
            result.TotalHours > 0);

        _attendanceRepositoryMock.Verify(
            x => x.UpdateAsync(
                It.IsAny<AttendanceEntity>()),
            Times.Once);

        _auditLogRepositoryMock.Verify(
            x => x.AddAsync(
                It.IsAny<WMS.Domain.Entities.AuditLog>()),
            Times.Once);
    }

    [Fact]
    public async Task GetMyAttendanceAsync_ReturnsRecords()
    {
        var attendances =
            new List<AttendanceEntity>
            {
                new()
                {
                    AttendanceId = 1,
                    EmpId = 1,
                    CheckIn =
                        DateTime.UtcNow
                }
            };

        _attendanceRepositoryMock
            .Setup(x =>
                x.GetByEmployeeIdAsync(1))
            .ReturnsAsync(
                attendances);

        var result =
            await _attendanceService
                .GetMyAttendanceAsync(
                    1);

        Assert.Single(result);
    }

    [Fact]
    public async Task GetMonthlyAttendanceAsync_ReturnsRecords()
    {
        var attendances =
            new List<AttendanceEntity>
            {
                new()
                {
                    AttendanceId = 1,
                    EmpId = 1,
                    CheckIn =
                        DateTime.UtcNow
                }
            };

        _attendanceRepositoryMock
            .Setup(x =>
                x.GetMonthlyAttendanceAsync(
                    1,
                    2026,
                    6))
            .ReturnsAsync(
                attendances);

        var result =
            await _attendanceService
                .GetMonthlyAttendanceAsync(
                    1,
                    2026,
                    6);

        Assert.Single(result);
    }
}