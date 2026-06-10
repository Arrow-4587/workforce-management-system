using WMS.Application.DTOs.Auth;
using WMS.Application.Services.JWT;
using WMS.Domain.Interfaces;

namespace WMS.Application.Services.Auth;

public class AuthService : IAuthService
{
    private readonly IAuthRepository _authRepository;
    private readonly IJwtService _jwtService;

    public AuthService(
    IAuthRepository authRepository,
    IJwtService jwtService)
    {
        _authRepository = authRepository;
        _jwtService = jwtService;
    }

    public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request)
    {
        var user = await _authRepository
     .GetByUsernameAsync(request.Username);

        if (user == null)
            return null;

        bool isValidPassword =
            BCrypt.Net.BCrypt.Verify(
                request.Password,
                user.PasswordHash);

        if (!isValidPassword)
            return null;
        user.LastLogin = DateTime.UtcNow;

        await _authRepository
            .UpdateAsync(user);

        return new LoginResponseDto
        {
            Username = user.Username,
            Role = user.Role?.RoleName ?? string.Empty,
            IsFirstLogin =
        user.IsFirstLogin,
            Token = _jwtService.GenerateToken(
    user.UserId,
    user.EmployeeId,
    user.Username,
    user.Role?.RoleName ?? string.Empty)
        };
    }
    public async Task ChangePasswordAsync(
    int userId,
    ChangePasswordDto dto)
    {
        var user =
            await _authRepository
                .GetByUserIdAsync(userId);

        if (user == null)
        {
            throw new Exception(
                "User not found.");
        }

        bool validPassword =
            BCrypt.Net.BCrypt.Verify(
                dto.CurrentPassword,
                user.PasswordHash);

        if (!validPassword)
        {
            throw new Exception(
                "Current password is incorrect.");
        }

        if (dto.CurrentPassword ==
            dto.NewPassword)
        {
            throw new Exception(
                "New password must be different from current password.");
        }

        if (dto.NewPassword.Length < 8)
        {
            throw new Exception(
                "Password must be at least 8 characters long.");
        }

        if (!dto.NewPassword.Any(char.IsUpper))
        {
            throw new Exception(
                "Password must contain at least one uppercase letter.");
        }

        if (!dto.NewPassword.Any(char.IsLower))
        {
            throw new Exception(
                "Password must contain at least one lowercase letter.");
        }

        if (!dto.NewPassword.Any(char.IsDigit))
        {
            throw new Exception(
                "Password must contain at least one number.");
        }

        if (!dto.NewPassword.Any(ch =>
            !char.IsLetterOrDigit(ch)))
        {
            throw new Exception(
                "Password must contain at least one special character.");
        }

        user.PasswordHash =
            BCrypt.Net.BCrypt.HashPassword(
                dto.NewPassword);

        user.IsFirstLogin = false;

        await _authRepository
            .UpdateAsync(user);
    }
}
