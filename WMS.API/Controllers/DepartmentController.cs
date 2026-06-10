using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WMS.Application.DTOs.Department;
using WMS.Application.Services.Department;

namespace WMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DepartmentController : ControllerBase
{
    private readonly IDepartmentService
        _departmentService;

    public DepartmentController(
        IDepartmentService departmentService)
    {
        _departmentService =
            departmentService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result =
            await _departmentService
                .GetAllAsync();

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(
        int id)
    {
        var result =
            await _departmentService
                .GetByIdAsync(id);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create(
        CreateDepartmentDto dto)
    {
        var result =
            await _departmentService
                .CreateAsync(dto);

        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        int id,
        UpdateDepartmentDto dto)
    {
        await _departmentService
            .UpdateAsync(id, dto);

        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(
        int id)
    {
        await _departmentService
            .DeleteAsync(id);

        return NoContent();
    }
    [HttpGet("search")]
    public async Task<IActionResult>
    SearchByName(string name)
    {
        var result =
            await _departmentService
                .SearchByNameAsync(name);

        return Ok(result);
    }
}