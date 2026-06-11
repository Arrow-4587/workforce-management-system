using WMS.Application.DTOs.Leave;
using WMS.Domain.Interfaces;
using LeaveEntity =
    WMS.Domain.Entities.Leave;

namespace WMS.Application.Services.Leave;

public class LeaveService
    : ILeaveService
{
    private readonly ILeaveRepository
        _leaveRepository;

    public LeaveService(
        ILeaveRepository leaveRepository)
    {
        _leaveRepository =
            leaveRepository;
    }

    public async Task<LeaveResponseDto>
        ApplyLeaveAsync(
            int employeeId,
            ApplyLeaveDto dto)
    {
        if (dto.FromDate > dto.ToDate)
        {
            throw new Exception(
                "FromDate cannot be later than ToDate.");
        }

        if (dto.FromDate.Date <
            DateTime.Today)
        {
            throw new Exception(
                "Cannot apply leave in the past.");
        }

        var leave =
            new LeaveEntity
            {
                EmpId = employeeId,
                LeaveType = dto.LeaveType,
                Reason = dto.Reason,
                FromDate = dto.FromDate,
                ToDate = dto.ToDate,
                Status = "Pending",
                AppliedOn = DateTime.UtcNow
            };

        await _leaveRepository
            .AddAsync(leave);

        return Map(leave);
    }

    public async Task CancelLeaveAsync(
        int employeeId,
        int leaveId)
    {
        var leave =
            await _leaveRepository
                .GetByIdAsync(leaveId);

        if (leave == null)
        {
            throw new Exception(
                "Leave not found.");
        }

        if (leave.EmpId != employeeId)
        {
            throw new Exception(
                "Unauthorized.");
        }

        if (leave.Status != "Pending")
        {
            throw new Exception(
                "Only pending leave can be cancelled.");
        }

        leave.Status = "Cancelled";

        await _leaveRepository
            .UpdateAsync(leave);
    }

    public async Task<
        List<LeaveResponseDto>>
        GetMyLeavesAsync(
            int employeeId)
    {
        var leaves =
            await _leaveRepository
                .GetByEmployeeIdAsync(
                    employeeId);

        return leaves
            .Select(Map)
            .ToList();
    }

    public async Task<
        List<LeaveResponseDto>>
        GetPendingLeavesAsync()
    {
        var leaves =
            await _leaveRepository
                .GetPendingLeavesAsync();

        return leaves
            .Select(Map)
            .ToList();
    }

    public async Task ApproveLeaveAsync(
        int leaveId,
        int managerId)
    {
        var leave =
            await _leaveRepository
                .GetByIdAsync(leaveId);

        if (leave == null)
        {
            throw new Exception(
                "Leave not found.");
        }

        if (leave.Status != "Pending")
        {
            throw new Exception(
                "Leave already processed.");
        }

        leave.Status = "Approved";
        leave.ApprovedBy = managerId;
        leave.ApprovedOn =
            DateTime.UtcNow;

        await _leaveRepository
            .UpdateAsync(leave);
    }

    public async Task RejectLeaveAsync(
        int leaveId,
        int managerId)
    {
        var leave =
            await _leaveRepository
                .GetByIdAsync(leaveId);

        if (leave == null)
        {
            throw new Exception(
                "Leave not found.");
        }

        if (leave.Status != "Pending")
        {
            throw new Exception(
                "Leave already processed.");
        }

        leave.Status = "Rejected";
        leave.ApprovedBy = managerId;
        leave.ApprovedOn =
            DateTime.UtcNow;

        await _leaveRepository
            .UpdateAsync(leave);
    }

    private static LeaveResponseDto
        Map(
            LeaveEntity leave)
    {
        return new LeaveResponseDto
        {
            LeaveId = leave.LeaveId,
            LeaveType = leave.LeaveType,
            Reason = leave.Reason,
            FromDate = leave.FromDate,
            ToDate = leave.ToDate,
            Status = leave.Status,
            AppliedOn = leave.AppliedOn,
            ApprovedBy = leave.ApprovedBy,
            ApprovedOn = leave.ApprovedOn
        };
    }
}