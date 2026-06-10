using Microsoft.AspNetCore.Mvc;
using WMS.Application.DTOs.Auth;
using WMS.Application.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace WMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequestDto request)
    {
        var result = await _authService.LoginAsync(request);

        if (result == null)
            return Unauthorized("Invalid username or password");

        return Ok(result);
    }
    [Authorize]
    [HttpGet("profile")]
    public IActionResult GetProfile()
    {
        return Ok(new
        {
            UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
            Username = User.Identity?.Name,
            Role = User.FindFirst(ClaimTypes.Role)?.Value
        });
    }
    [Authorize]
    [HttpPost("change-password")]
    public async Task<IActionResult>
    ChangePassword(
        ChangePasswordDto dto)
    {
        var userIdClaim =
            User.FindFirst(
                ClaimTypes.NameIdentifier);

        if (userIdClaim == null)
            return Unauthorized();

        int userId =
            int.Parse(
                userIdClaim.Value);

        await _authService
            .ChangePasswordAsync(
                userId,
                dto);

        return Ok(
            "Password changed successfully.");
    }
}