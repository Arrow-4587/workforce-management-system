using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WMS.Application.DTOs.Leave;
using WMS.Application.Services.Leave;

namespace WMS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class LeaveController : ControllerBase
{
    private readonly ILeaveService
        _leaveService;

    public LeaveController(
        ILeaveService leaveService)
    {
        _leaveService = leaveService;
    }

    [HttpPost("apply")]
    public async Task<IActionResult>
        ApplyLeave(
            ApplyLeaveDto dto)
    {
        if (User.IsInRole("Admin"))
        {
            return Forbid("Admins cannot apply for leave.");
        }

        int employeeId =
            GetEmployeeId();

        var result =
            await _leaveService
                .ApplyLeaveAsync(
                    employeeId,
                    dto);

        return Ok(result);
    }

    [HttpDelete("cancel/{leaveId}")]
    public async Task<IActionResult>
        CancelLeave(
            int leaveId)
    {
        int employeeId =
            GetEmployeeId();

        await _leaveService
            .CancelLeaveAsync(
                employeeId,
                leaveId);

        return Ok(
            "Leave cancelled successfully.");
    }

    [HttpGet("my")]
    public async Task<IActionResult>
        GetMyLeaves()
    {
        int employeeId =
            GetEmployeeId();

        var result =
            await _leaveService
                .GetMyLeavesAsync(
                    employeeId);

        return Ok(result);
    }

[Authorize(Roles = "Admin,Manager")]
[HttpGet("pending")]
public async Task<IActionResult>
    GetPendingLeaves()
{
    if (User.IsInRole("Admin"))
    {
        return Ok(
            await _leaveService
                .GetPendingLeavesAsync());
    }

    int managerId =
        GetEmployeeId();

    return Ok(
        await _leaveService
            .GetPendingLeavesForManagerAsync(
                managerId));
}

    [Authorize(Roles = "Admin,Manager")]
    [HttpPost("approve/{leaveId}")]
    public async Task<IActionResult>
        ApproveLeave(
            int leaveId)
    {
        int? managerId = GetEmployeeIdOrNull();
        int userId = GetUserId();

        await _leaveService
            .ApproveLeaveAsync(
                leaveId,
                managerId,
                userId);

        return Ok(
            "Leave approved.");
    }

    [Authorize(Roles = "Admin,Manager")]
    [HttpPost("reject/{leaveId}")]
    public async Task<IActionResult>
        RejectLeave(
            int leaveId)
    {
        int? managerId = GetEmployeeIdOrNull();
        int userId = GetUserId();

        await _leaveService
            .RejectLeaveAsync(
                leaveId,
                managerId,
                userId);

        return Ok(
            "Leave rejected.");
    }

    private int GetEmployeeId()
    {
        var claim =
            User.FindFirst("EmployeeId") ?? User.FindFirst(c => c.Type.Equals("EmployeeId", StringComparison.OrdinalIgnoreCase));

        if (claim == null)
        {
            var claims = string.Join(", ", User.Claims.Select(c => c.Type + "=" + c.Value));
            throw new Exception(
                $"EmployeeId claim not found. Available claims: {claims}");
        }

        return int.Parse(
            claim.Value);
    }

    private int? GetEmployeeIdOrNull()
    {
        var claim = User.FindFirst("EmployeeId");
        if (claim == null)
            return null;

        return int.Parse(claim.Value);
    }

    private int GetUserId()
    {
        var claim =
            User.FindFirst(
                ClaimTypes.NameIdentifier);

        if (claim == null)
        {
            throw new Exception(
                "UserId claim not found.");
        }

        return int.Parse(
            claim.Value);
    }
}