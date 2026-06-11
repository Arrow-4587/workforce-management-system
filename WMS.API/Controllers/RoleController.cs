using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WMS.Application.DTOs.Role;
using WMS.Application.Services;

namespace WMS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]
public class RoleController
    : ControllerBase
{
    private readonly IRoleService
        _roleService;

    public RoleController(
        IRoleService roleService)
    {
        _roleService =
            roleService;
    }

    [HttpGet]
    //[Authorize]
    public async Task<IActionResult>
        GetAll()
    {
        return Ok(
            await _roleService
                .GetAllAsync());
    }

    [HttpGet("{id}")]
    //[Authorize]
    public async Task<IActionResult>
        GetById(
            int id)
    {
        return Ok(
            await _roleService
                .GetByIdAsync(id));
    }

    [HttpPost]
    //[Authorize(Roles = "Admin")]
    public async Task<IActionResult>
        Create(
            CreateRoleDto dto)
    {
        return Ok(
            await _roleService
                .CreateAsync(dto));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult>
        Update(
            int id,
            UpdateRoleDto dto)
    {
        await _roleService
            .UpdateAsync(
                id,
                dto);

        return Ok(
            "Role updated successfully.");
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult>
        Delete(
            int id)
    {
        await _roleService
            .DeleteAsync(id);

        return Ok(
            "Role deleted successfully.");
    }
}