using WMS.Domain.Entities;

namespace WMS.Domain.Interfaces;

public interface IAuditLogRepository
{
    Task AddAsync(
        AuditLog auditLog);

    Task<List<AuditLog>>
        GetAllAsync();

    Task<List<AuditLog>>
        GetByEntityAsync(
            string entityName);
}