using WMS.Application.DTOs.Announcement;
using WMS.Domain.Entities;
using WMS.Domain.Interfaces;

namespace WMS.Application.Services;

public class AnnouncementService
    : IAnnouncementService
{
    private readonly IAnnouncementRepository
        _announcementRepository;
    

    private readonly IAuditLogRepository
        _auditLogRepository;

    public AnnouncementService(
     IAnnouncementRepository announcementRepository,
     IAuditLogRepository auditLogRepository)
    {
        _announcementRepository =
            announcementRepository;

        _auditLogRepository =
            auditLogRepository;
    }

    public async Task<List<AnnouncementResponseDto>>
        GetAllAsync()
    {
        var announcements =
            await _announcementRepository
                .GetAllAsync();

        return announcements
            .Select(Map)
            .ToList();
    }

    public async Task<AnnouncementResponseDto>
        GetByIdAsync(
            int announcementId)
    {
        var announcement =
            await _announcementRepository
                .GetByIdAsync(announcementId);

        if (announcement == null)
        {
            throw new Exception(
                "Announcement not found.");
        }

        return Map(announcement);
    }

    public async Task<AnnouncementResponseDto>
        CreateAsync(
            int userId,
            CreateAnnouncementDto dto)
    {
        var announcement =
            new Announcement
            {
                Title =
                    dto.Title,

                Message =
                    dto.Message,

                CreatedBy =
                    userId,

                CreatedOn =
                    DateTime.UtcNow,

                IsActive = true
            };

        await _announcementRepository
            .AddAsync(announcement);
        await _auditLogRepository
    .AddAsync(
        new AuditLog
        {
            EntityName = "Announcement",
            RecordId =
                announcement.AnnouncementId,
            Action = "Insert",
            CreatedBy =
                announcement.CreatedBy,
            CreatedOn =
                DateTime.UtcNow
        });

        announcement =
            await _announcementRepository
                .GetByIdAsync(
                    announcement.AnnouncementId)
            ?? announcement;

        return Map(announcement);
    }

    public async Task UpdateAsync(
        int announcementId,
        UpdateAnnouncementDto dto)
    {
        var announcement =
            await _announcementRepository
                .GetByIdAsync(
                    announcementId);

        if (announcement == null)
        {
            throw new Exception(
                "Announcement not found.");
        }

        announcement.Title =
            dto.Title;

        announcement.Message =
            dto.Message;

        announcement.IsActive =
            dto.IsActive;

        await _announcementRepository
            .UpdateAsync(announcement);
        await _auditLogRepository
    .AddAsync(
        new AuditLog
        {
            EntityName = "Announcement",
            RecordId =
                announcement.AnnouncementId,
            Action = "Update",
            CreatedBy =
                announcement.CreatedBy,
            CreatedOn =
                DateTime.UtcNow
        });
    }

    public async Task DeleteAsync(
        int announcementId)
    {
        var announcement =
            await _announcementRepository
                .GetByIdAsync(
                    announcementId);

        if (announcement == null)
        {
            throw new Exception(
                "Announcement not found.");
        }

        await _announcementRepository
            .DeleteAsync(announcement);
        await _auditLogRepository
    .AddAsync(
        new AuditLog
        {
            EntityName = "Announcement",
            RecordId =
                announcement.AnnouncementId,
            Action = "Delete",
            CreatedBy =
                announcement.CreatedBy,
            CreatedOn =
                DateTime.UtcNow
        });
    }

    private static AnnouncementResponseDto
        Map(
            Announcement announcement)
    {
        return new AnnouncementResponseDto
        {
            AnnouncementId =
                announcement.AnnouncementId,

            Title =
                announcement.Title,

            Message =
                announcement.Message,

            CreatedBy =
                announcement.CreatedBy,

            CreatedByName =
                announcement.Creator?.Username
                ?? string.Empty,

            CreatedOn =
                announcement.CreatedOn,

            IsActive =
                announcement.IsActive
        };
    }
}