using WMS.Application.DTOs.AuditLog;

namespace WMS.Application.Services;

public interface IAuditLogService
{
    Task LogAsync(
        string entityName,
        int recordId,
        string action,
        int userId);

    Task<List<AuditLogResponseDto>>
        GetAllAsync();

    Task<List<AuditLogResponseDto>>
        GetByEntityAsync(
            string entityName);
}