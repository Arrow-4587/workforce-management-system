using WMS.Application.DTOs.AuditLog;
using WMS.Domain.Entities;
using WMS.Domain.Interfaces;

namespace WMS.Application.Services;

public class AuditLogService
    : IAuditLogService
{
    private readonly IAuditLogRepository
        _auditLogRepository;

    public AuditLogService(
        IAuditLogRepository auditLogRepository)
    {
        _auditLogRepository =
            auditLogRepository;
    }

    public async Task LogAsync(
        string entityName,
        int recordId,
        string action,
        int userId)
    {
        var auditLog =
            new AuditLog
            {
                EntityName =
                    entityName,

                RecordId =
                    recordId,

                Action =
                    action,

                CreatedBy =
                    userId,

                CreatedOn =
                    DateTime.UtcNow
            };

        await _auditLogRepository
            .AddAsync(auditLog);
    }

    public async Task<List<AuditLogResponseDto>>
        GetAllAsync()
    {
        var logs =
            await _auditLogRepository
                .GetAllAsync();

        return logs
            .Select(Map)
            .ToList();
    }

    public async Task<List<AuditLogResponseDto>>
        GetByEntityAsync(
            string entityName)
    {
        var logs =
            await _auditLogRepository
                .GetByEntityAsync(
                    entityName);

        return logs
            .Select(Map)
            .ToList();
    }

    private static AuditLogResponseDto
        Map(
            AuditLog log)
    {
        return new AuditLogResponseDto
        {
            AuditId =
                log.AuditId,

            EntityName =
                log.EntityName,

            RecordId =
                log.RecordId,

            Action =
                log.Action,

            CreatedBy =
                log.CreatedBy,

            Username =
                log.User?.Username
                ?? string.Empty,

            CreatedOn =
                log.CreatedOn
        };
    }
}