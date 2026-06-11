using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WMS.Application.DTOs.Project;
using WMS.Application.Services.Project;
using System.Security.Claims;

namespace WMS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin,Manager")]
public class ProjectController : ControllerBase
{
    private readonly IProjectService
        _projectService;

    public ProjectController(
        IProjectService projectService)
    {
        _projectService =
            projectService;
    }

    [HttpGet]
    public async Task<IActionResult>
        GetAll()
    {
        return Ok(
            await _projectService
                .GetAllAsync());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult>
        GetById(
            int id)
    {
        return Ok(
            await _projectService
                .GetByIdAsync(id));
    }
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult>
        Create(
            CreateProjectDto dto)
    {
        return Ok(
            await _projectService
                .CreateAsync(dto));
    }
    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult>
        Update(
            int id,
            UpdateProjectDto dto)
    {
        return Ok(
            await _projectService
                .UpdateAsync(id, dto));
    }
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult>
        Delete(
            int id)
    {
        await _projectService
            .DeleteAsync(id);

        return Ok(
            "Project deleted successfully.");
    }

    [Authorize(Roles = "Manager")]
    [HttpGet("my-projects")]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult>
    GetMyProjects()
    {
        var employeeId =
            int.Parse(
                User.FindFirst(
                    "EmployeeId")!.Value);

        return Ok(
            await _projectService
                .GetMyProjectsAsync(
                    employeeId));
    }
}