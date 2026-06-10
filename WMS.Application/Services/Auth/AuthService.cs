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

        return new LoginResponseDto
        {
            Username = user.Username,
            Role = user.Role?.RoleName ?? string.Empty,
            Token = _jwtService.GenerateToken(
    user.UserId,
    user.Username,
    user.Role?.RoleName ?? string.Empty)
        };
    }
}
