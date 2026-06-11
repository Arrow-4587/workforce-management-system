using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WMS.Application.Services.Dashboard;

namespace WMS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class DashboardController
    : ControllerBase
{
    private readonly IDashboardService
        _dashboardService;

    public DashboardController(
        IDashboardService dashboardService)
    {
        _dashboardService =
            dashboardService;
    }

    [HttpGet("admin")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult>
        GetAdminDashboard()
    {
        return Ok(
            await _dashboardService
                .GetAdminDashboardAsync());
    }

    [HttpGet("manager")]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult>
        GetManagerDashboard()
    {
        int employeeId =
            GetEmployeeId();

        return Ok(
            await _dashboardService
                .GetManagerDashboardAsync(
                    employeeId));
    }

    [HttpGet("employee")]
    [Authorize(Roles = "Employee")]
    public async Task<IActionResult>
        GetEmployeeDashboard()
    {
        int employeeId =
            GetEmployeeId();

        return Ok(
            await _dashboardService
                .GetEmployeeDashboardAsync(
                    employeeId));
    }

    private int GetEmployeeId()
    {
        var claim =
            User.FindFirst(
                "EmployeeId");

        if (claim == null)
        {
            throw new Exception(
                "EmployeeId claim not found.");
        }

        return int.Parse(
            claim.Value);
    }
}