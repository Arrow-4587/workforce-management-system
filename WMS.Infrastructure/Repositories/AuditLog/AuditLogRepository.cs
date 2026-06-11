using Microsoft.EntityFrameworkCore;
using WMS.Domain.Entities;
using WMS.Domain.Interfaces;
using WMS.Infrastructure.Data;

namespace WMS.Infrastructure.Repositories;

public class AuditLogRepository
    : IAuditLogRepository
{
    private readonly WmsDbContext _context;

    public AuditLogRepository(
        WmsDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(
        AuditLog auditLog)
    {
        await _context.AuditLogs
            .AddAsync(auditLog);

        await _context.SaveChangesAsync();
    }

    public async Task<List<AuditLog>>
        GetAllAsync()
    {
        return await _context.AuditLogs
            .Include(a => a.User)
            .OrderByDescending(a =>
                a.CreatedOn)
            .ToListAsync();
    }

    public async Task<List<AuditLog>>
        GetByEntityAsync(
            string entityName)
    {
        return await _context.AuditLogs
            .Include(a => a.User)
            .Where(a =>
                a.EntityName == entityName)
            .OrderByDescending(a =>
                a.CreatedOn)
            .ToListAsync();
    }
}