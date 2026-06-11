using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WMS.Application.DTOs.Allocation;
using WMS.Application.Services.Allocation;

namespace WMS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin,Manager")]
public class AllocationController : ControllerBase
{
    private readonly IAllocationService
        _allocationService;

    public AllocationController(
        IAllocationService allocationService)
    {
        _allocationService =
            allocationService;
    }
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult>
        Allocate(
            AllocateEmployeeDto dto)
    {
        return Ok(
            await _allocationService
                .AllocateAsync(dto));
    }
    [Authorize(Roles = "Admin")]
    [HttpDelete("{allocationId}")]
    public async Task<IActionResult>
        Release(
            int allocationId)
    {
        await _allocationService
            .ReleaseAsync(
                allocationId);

        return Ok(
            "Employee released successfully.");
    }

    [HttpGet("project/{projectId}")]
    public async Task<IActionResult>
        GetByProject(
            int projectId)
    {
        return Ok(
            await _allocationService
                .GetByProjectAsync(
                    projectId));
    }

    [HttpGet("employee/{employeeId}")]
    public async Task<IActionResult>
        GetByEmployee(
            int employeeId)
    {
        return Ok(
            await _allocationService
                .GetByEmployeeAsync(
                    employeeId));
    }
    [HttpGet("my-projects")]
    [Authorize(Roles = "Employee")]
    public async Task<IActionResult>
    GetMyProjects()
    {
        var employeeId =
            int.Parse(
                User.FindFirst(
                    "EmployeeId")!.Value);

        return Ok(
            await _allocationService
                .GetMyProjectsAsync(
                    employeeId));
    }
}