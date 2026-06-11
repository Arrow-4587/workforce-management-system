using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WMS.Application.DTOs.Attendance;
using WMS.Application.Services.Attendance;

namespace WMS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class AttendanceController : ControllerBase
{
    private readonly IAttendanceService
        _attendanceService;

    public AttendanceController(
        IAttendanceService attendanceService)
    {
        _attendanceService =
            attendanceService;
    }
    
    [HttpPost("checkin")]
    public async Task<IActionResult>
        CheckIn(
            CheckInDto dto)
    {
        int employeeId =
            GetEmployeeId();

        var result =
            await _attendanceService
                .CheckInAsync(
                    employeeId,
                    dto);

        return Ok(result);
    }
    
    [HttpPost("checkout")]
    public async Task<IActionResult>
        CheckOut()
    {
        int employeeId =
            GetEmployeeId();

        var result =
            await _attendanceService
                .CheckOutAsync(
                    employeeId);

        return Ok(result);
    }

    [HttpGet("my")]
    public async Task<IActionResult>
        GetMyAttendance()
    {
        int employeeId =
            GetEmployeeId();

        var result =
            await _attendanceService
                .GetMyAttendanceAsync(
                    employeeId);

        return Ok(result);
    }
    [Authorize(Roles = "Admin,Manager")]
    [HttpGet("employee/{employeeId}")]
    public async Task<IActionResult>
    GetEmployeeAttendance(
        int employeeId)
    {
        var result =
            await _attendanceService
                .GetEmployeeAttendanceAsync(
                    employeeId);

        return Ok(result);
    }

    [HttpGet("monthly")]
    public async Task<IActionResult>
    GetMonthlyAttendance(
        int year,
        int month)
    {
        int employeeId =
            GetEmployeeId();

        var result =
            await _attendanceService
                .GetMonthlyAttendanceAsync(
                    employeeId,
                    year,
                    month);

        return Ok(result);
    }
    private int GetEmployeeId()
    {
        var employeeIdClaim =
            User.FindFirst(
                "EmployeeId");

        if (employeeIdClaim == null)
        {
            throw new Exception(
                "EmployeeId claim not found.");
        }

        return int.Parse(
            employeeIdClaim.Value);
    }
}