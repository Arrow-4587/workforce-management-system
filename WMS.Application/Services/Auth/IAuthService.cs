using WMS.Application.DTOs.Auth;
using WMS.Domain.Interfaces;

namespace WMS.Application.Services.Auth;

public interface IAuthService
{
    Task<LoginResponseDto?> LoginAsync(LoginRequestDto request);
}