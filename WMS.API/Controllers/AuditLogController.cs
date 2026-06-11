using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WMS.Application.Services;

namespace WMS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]
public class AuditLogController : ControllerBase
{
    private readonly IAuditLogService
        _auditLogService;

    public AuditLogController(
        IAuditLogService auditLogService)
    {
        _auditLogService =
            auditLogService;
    }

    [HttpGet]
    public async Task<IActionResult>
        GetAll()
    {
        return Ok(
            await _auditLogService
                .GetAllAsync());
    }

    [HttpGet("entity/{entityName}")]
    public async Task<IActionResult>
        GetByEntity(
            string entityName)
    {
        return Ok(
            await _auditLogService
                .GetByEntityAsync(
                    entityName));
    }
}