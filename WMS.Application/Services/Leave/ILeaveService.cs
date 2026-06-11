using WMS.Application.DTOs.Leave;

namespace WMS.Application.Services.Leave;

public interface ILeaveService
{
    Task<LeaveResponseDto>
        ApplyLeaveAsync(
            int employeeId,
            ApplyLeaveDto dto);

    Task CancelLeaveAsync(
        int employeeId,
        int leaveId);

    Task<List<LeaveResponseDto>>
        GetMyLeavesAsync(
            int employeeId);

    Task<List<LeaveResponseDto>>
        GetPendingLeavesAsync();

    Task ApproveLeaveAsync(
        int leaveId,
        int managerId);

    Task RejectLeaveAsync(
        int leaveId,
        int managerId);
}