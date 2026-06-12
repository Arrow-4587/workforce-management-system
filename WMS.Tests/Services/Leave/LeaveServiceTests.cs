using Moq;
using WMS.Application.DTOs.Leave;
using WMS.Application.Services.Leave;
using WMS.Domain.Entities;
using WMS.Domain.Interfaces;
using Xunit;

namespace WMS.Tests.Services.Leave;

public class LeaveServiceTests
{
    private readonly Mock<ILeaveRepository> _leaveRepositoryMock;
    private readonly Mock<IEmployeeProjectRepository> _employeeProjectRepositoryMock;
    private readonly Mock<IAuditLogRepository> _auditLogRepositoryMock;

    private readonly LeaveService _leaveService;

    public LeaveServiceTests()
    {
        _leaveRepositoryMock =
            new Mock<ILeaveRepository>();

        _employeeProjectRepositoryMock =
            new Mock<IEmployeeProjectRepository>();

        _auditLogRepositoryMock =
            new Mock<IAuditLogRepository>();

        _leaveService =
            new LeaveService(
                _leaveRepositoryMock.Object,
                _employeeProjectRepositoryMock.Object,
                _auditLogRepositoryMock.Object);
    }

    [Fact]
    public async Task ApplyLeaveAsync_ValidRequest_ReturnsPendingStatus()
    {
        // Arrange
        var dto = new ApplyLeaveDto
        {
            LeaveType = "Casual",
            Reason = "Personal Work",
            FromDate = DateTime.Today.AddDays(1),
            ToDate = DateTime.Today.AddDays(2)
        };

        // Act
        var result =
            await _leaveService.ApplyLeaveAsync(
                1,
                dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(
            "Pending",
            result.Status);

        _leaveRepositoryMock.Verify(
            x => x.AddAsync(
                It.IsAny<WMS.Domain.Entities.Leave>()),
            Times.Once);

        _auditLogRepositoryMock.Verify(
            x => x.AddAsync(
                It.IsAny<AuditLog>()),
            Times.Once);
    }

    [Fact]
    public async Task ApplyLeaveAsync_FromDateGreaterThanToDate_ThrowsException()
    {
        // Arrange
        var dto = new ApplyLeaveDto
        {
            LeaveType = "Casual",
            Reason = "Test",
            FromDate = DateTime.Today.AddDays(5),
            ToDate = DateTime.Today.AddDays(1)
        };

        // Act & Assert
        var ex = await Assert.ThrowsAsync<Exception>(
            () => _leaveService.ApplyLeaveAsync(
                1,
                dto));

        Assert.Equal(
            "FromDate cannot be later than ToDate.",
            ex.Message);
    }

    [Fact]
    public async Task ApplyLeaveAsync_PastDate_ThrowsException()
    {
        // Arrange
        var dto = new ApplyLeaveDto
        {
            LeaveType = "Casual",
            Reason = "Test",
            FromDate = DateTime.Today.AddDays(-1),
            ToDate = DateTime.Today.AddDays(1)
        };

        // Act & Assert
        var ex = await Assert.ThrowsAsync<Exception>(
            () => _leaveService.ApplyLeaveAsync(
                1,
                dto));

        Assert.Equal(
            "Cannot apply leave in the past.",
            ex.Message);
    }

    [Fact]
    public async Task CancelLeaveAsync_LeaveNotFound_ThrowsException()
    {
        // Arrange
        _leaveRepositoryMock
            .Setup(x => x.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((WMS.Domain.Entities.Leave?)null);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<Exception>(
            () => _leaveService.CancelLeaveAsync(
                1,
                1));

        Assert.Equal(
            "Leave not found.",
            ex.Message);
    }

    [Fact]
    public async Task CancelLeaveAsync_UnauthorizedEmployee_ThrowsException()
    {
        // Arrange
        var leave = new WMS.Domain.Entities.Leave
        {
            LeaveId = 1,
            EmpId = 2,
            Status = "Pending"
        };

        _leaveRepositoryMock
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(leave);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<Exception>(
            () => _leaveService.CancelLeaveAsync(
                1,
                1));

        Assert.Equal(
            "Unauthorized.",
            ex.Message);
    }

    [Fact]
    public async Task ApproveLeaveAsync_UnauthorizedManager_ThrowsException()
    {
        // Arrange
        var leave = new WMS.Domain.Entities.Leave
        {
            LeaveId = 1,
            EmpId = 10,
            Status = "Pending"
        };

        _leaveRepositoryMock
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(leave);

        _employeeProjectRepositoryMock
            .Setup(x => x.IsEmployeeUnderManagerAsync(
                leave.EmpId,
                99))
            .ReturnsAsync(false);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<Exception>(
            () => _leaveService.ApproveLeaveAsync(
                1,
                99,
                99));

        Assert.Equal(
            "You are not authorized to approve this employee's leave.",
            ex.Message);
    }

    [Fact]
    public async Task ApproveLeaveAsync_ValidRequest_UpdatesLeave()
    {
        // Arrange
        var leave = new WMS.Domain.Entities.Leave
        {
            LeaveId = 1,
            EmpId = 10,
            Status = "Pending"
        };

        _leaveRepositoryMock
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(leave);

        _employeeProjectRepositoryMock
            .Setup(x => x.IsEmployeeUnderManagerAsync(
                leave.EmpId,
                99))
            .ReturnsAsync(true);

        // Act
        await _leaveService.ApproveLeaveAsync(
            1,
            99,
            99);

        // Assert
        Assert.Equal(
            "Approved",
            leave.Status);

        _leaveRepositoryMock.Verify(
            x => x.UpdateAsync(
                It.IsAny<WMS.Domain.Entities.Leave>()),
            Times.Once);

        _auditLogRepositoryMock.Verify(
            x => x.AddAsync(
                It.IsAny<AuditLog>()),
            Times.Once);
    }

    [Fact]
    public async Task RejectLeaveAsync_ValidRequest_UpdatesLeave()
    {
        // Arrange
        var leave = new WMS.Domain.Entities.Leave
        {
            LeaveId = 1,
            EmpId = 10,
            Status = "Pending"
        };

        _leaveRepositoryMock
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(leave);

        _employeeProjectRepositoryMock
            .Setup(x => x.IsEmployeeUnderManagerAsync(
                leave.EmpId,
                99))
            .ReturnsAsync(true);

        // Act
        await _leaveService.RejectLeaveAsync(
            1,
            99,
            99);

        // Assert
        Assert.Equal(
            "Rejected",
            leave.Status);

        _leaveRepositoryMock.Verify(
            x => x.UpdateAsync(
                It.IsAny<WMS.Domain.Entities.Leave>()),
            Times.Once);

        _auditLogRepositoryMock.Verify(
            x => x.AddAsync(
                It.IsAny<AuditLog>()),
            Times.Once);
    }
}