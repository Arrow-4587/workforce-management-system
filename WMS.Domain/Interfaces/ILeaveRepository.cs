using WMS.Domain.Entities;

namespace WMS.Domain.Interfaces;

public interface ILeaveRepository
{
    Task AddAsync(
        Leave leave);

    Task UpdateAsync(
        Leave leave);

    Task<Leave?> GetByIdAsync(
        int leaveId);

    Task<List<Leave>>
        GetByEmployeeIdAsync(
            int employeeId);

    Task<List<Leave>>
        GetPendingLeavesAsync();
        Task<int> GetPendingCountAsync();

    Task<List<Leave>> GetAllAsync();
}