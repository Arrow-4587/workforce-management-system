using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WMS.Application.DTOs.Profile;
using WMS.Application.Services.Profile;

namespace WMS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ProfileController : ControllerBase
{
    private readonly IProfileService
        _profileService;

    public ProfileController(
        IProfileService profileService)
    {
        _profileService =
            profileService;
    }

    [HttpGet]
    public async Task<IActionResult>
        GetProfile()
    {
        int userId =
            int.Parse(
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier)!);

        return Ok(
            await _profileService
                .GetProfileAsync(userId));
    }

    [HttpPut]
    public async Task<IActionResult>
        UpdateProfile(
            UpdateProfileDto dto)
    {
        int userId =
            int.Parse(
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier)!);

        await _profileService
            .UpdateProfileAsync(
                userId,
                dto);

        return Ok(
            "Profile updated successfully.");
    }

    [HttpPut("change-password")]
    public async Task<IActionResult>
        ChangePassword(
            ChangePasswordDto dto)
    {
        int userId =
            int.Parse(
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier)!);

        await _profileService
            .ChangePasswordAsync(
                userId,
                dto);

        return Ok(
            "Password changed successfully.");
    }
}