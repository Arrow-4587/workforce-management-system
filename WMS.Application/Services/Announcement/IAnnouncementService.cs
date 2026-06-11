using WMS.Application.DTOs.Announcement;

namespace WMS.Application.Services;

public interface IAnnouncementService
{
    Task<List<AnnouncementResponseDto>>
        GetAllAsync();

    Task<AnnouncementResponseDto>
        GetByIdAsync(
            int announcementId);

    Task<AnnouncementResponseDto>
        CreateAsync(
            int userId,
            CreateAnnouncementDto dto);

    Task UpdateAsync(
        int announcementId,
        UpdateAnnouncementDto dto);

    Task DeleteAsync(
        int announcementId);
}