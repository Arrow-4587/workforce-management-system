using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WMS.Application.DTOs.Announcement;
using WMS.Application.Services;

namespace WMS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class AnnouncementController : ControllerBase
{
    private readonly IAnnouncementService
        _announcementService;

    public AnnouncementController(
        IAnnouncementService announcementService)
    {
        _announcementService =
            announcementService;
    }

    [HttpGet]
    public async Task<IActionResult>
     GetAll()
    {
        var announcements =
            await _announcementService
                .GetAllAsync();

        if (!User.IsInRole("Admin"))
        {
            announcements =
                announcements
                    .Where(a =>
                        a.IsActive)
                    .ToList();
        }

        return Ok(announcements);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult>
        GetById(
            int id)
    {
        return Ok(
            await _announcementService
                .GetByIdAsync(id));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult>
        Create(
            CreateAnnouncementDto dto)
    {
        int userId =
            int.Parse(
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier)!);

        return Ok(
            await _announcementService
                .CreateAsync(
                    userId,
                    dto));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult>
        Update(
            int id,
            UpdateAnnouncementDto dto)
    {
        await _announcementService
            .UpdateAsync(
                id,
                dto);

        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult>
        Delete(
            int id)
    {
        await _announcementService
            .DeleteAsync(id);

        return NoContent();
    }
}