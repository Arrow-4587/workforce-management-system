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
        try
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
        catch (Exception ex)
        {
            var claims = User.Claims.Select(c => $"{c.Type}: {c.Value}").ToList();
            return BadRequest(new { message = ex.Message, claims = claims });
        }
    }
    
    [HttpPost("checkout")]
    public async Task<IActionResult>
        CheckOut()
    {
        try
        {
            int employeeId =
                GetEmployeeId();

            var result =
                await _attendanceService
                    .CheckOutAsync(
                        employeeId);

            return Ok(result);
        }
        catch (Exception ex)
        {
            var claims = User.Claims.Select(c => $"{c.Type}: {c.Value}").ToList();
            return BadRequest(new { message = ex.Message, claims = claims });
        }
    }

    [HttpGet("my")]
    public async Task<IActionResult>
        GetMyAttendance()
    {
        try
        {
            int employeeId =
                GetEmployeeId();

            var result =
                await _attendanceService
                    .GetMyAttendanceAsync(
                        employeeId);

            return Ok(result);
        }
        catch (Exception ex)
        {
            var claims = User.Claims.Select(c => $"{c.Type}: {c.Value}").ToList();
            return BadRequest(new { message = ex.Message, claims = claims });
        }
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
            User.FindFirst("EmployeeId") ?? User.FindFirst(c => c.Type.Equals("EmployeeId", StringComparison.OrdinalIgnoreCase));

        if (employeeIdClaim == null)
        {
            var claims = string.Join(", ", User.Claims.Select(c => c.Type + "=" + c.Value));
            throw new Exception(
                $"EmployeeId claim not found. Available claims: {claims}");
        }

        return int.Parse(
            employeeIdClaim.Value);
    }
}