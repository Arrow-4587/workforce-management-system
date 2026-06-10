using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WMS.Application.DTOs.Employee;
using WMS.Application.Services.Employee;

namespace WMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EmployeeController : ControllerBase
{
    private readonly IEmployeeService _employeeService;

    public EmployeeController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _employeeService.GetAllAsync();

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _employeeService.GetByIdAsync(id);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create(
        CreateEmployeeDto dto)
    {
        var result =
            await _employeeService.CreateAsync(dto);

        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        int id,
        UpdateEmployeeDto dto)
    {
        await _employeeService.UpdateAsync(id, dto);

        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _employeeService.DeleteAsync(id);

        return NoContent();
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchByName(
        string name)
    {
        var result =
            await _employeeService.SearchByNameAsync(name);

        return Ok(result);
    }

    [HttpGet("department/{departmentId}")]
    public async Task<IActionResult> GetByDepartment(
        int departmentId)
    {
        var result =
            await _employeeService
                .GetByDepartmentAsync(departmentId);

        return Ok(result);
    }

    [HttpGet("role/{roleId}")]
    public async Task<IActionResult> GetByRole(
        int roleId)
    {
        var result =
            await _employeeService
                .GetByRoleAsync(roleId);

        return Ok(result);
    }
}

