using WMS.Application.DTOs.Profile;
using WMS.Application.DTOs.Auth;

namespace WMS.Application.Services.Profile;

public interface IProfileService
{
    Task<ProfileResponseDto>
        GetProfileAsync(
            int userId);

    Task UpdateProfileAsync(
        int userId,
        UpdateProfileDto dto);

    Task ChangePasswordAsync(
        int userId,
        ChangePasswordDto dto);
}
