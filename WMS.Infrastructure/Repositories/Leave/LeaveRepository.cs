using Microsoft.EntityFrameworkCore;
using WMS.Domain.Interfaces;
using WMS.Infrastructure.Data;
using LeaveEntity =
    WMS.Domain.Entities.Leave;

namespace WMS.Infrastructure.Repositories;

public class LeaveRepository
    : ILeaveRepository
{
    private readonly WmsDbContext _context;

    public LeaveRepository(
        WmsDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(
        LeaveEntity leave)
    {
        await _context.Leaves
            .AddAsync(leave);

        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(
        LeaveEntity leave)
    {
        _context.Leaves
            .Update(leave);

        await _context.SaveChangesAsync();
    }

    public async Task<LeaveEntity?>
        GetByIdAsync(
            int leaveId)
    {
        return await _context.Leaves
            .FirstOrDefaultAsync(l =>
                l.LeaveId == leaveId);
    }

    public async Task<List<LeaveEntity>>
        GetByEmployeeIdAsync(
            int employeeId)
    {
        return await _context.Leaves
            .Where(l =>
                l.EmpId == employeeId)
            .OrderByDescending(l =>
                l.AppliedOn)
            .ToListAsync();
    }

    public async Task<List<LeaveEntity>>
        GetPendingLeavesAsync()
    {
        return await _context.Leaves
            .Where(l =>
                l.Status == "Pending")
            .OrderBy(l =>
                l.AppliedOn)
            .ToListAsync();
    }
    public async Task<int>
    GetPendingCountAsync()
{
    return await _context.Leaves
        .CountAsync(l =>
            l.Status == "Pending");
}
}