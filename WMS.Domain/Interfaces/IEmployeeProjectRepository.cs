using WMS.Domain.Entities;

namespace WMS.Domain.Interfaces;

public interface IEmployeeProjectRepository
{
    Task AddAsync(
        EmployeeProject allocation);

    Task UpdateAsync(
        EmployeeProject allocation);

    Task<EmployeeProject?>
        GetByIdAsync(
            int allocationId);

    Task<List<EmployeeProject>>
        GetByProjectIdAsync(
            int projectId);

    Task<List<EmployeeProject>>
        GetByEmployeeIdAsync(
            int employeeId);
    Task<EmployeeProject?>
    GetActiveAllocationAsync(
        int employeeId,
        int projectId);

    Task<bool>
    IsEmployeeUnderManagerAsync(
        int employeeId,
        int managerId);
        Task<List<int>>
    GetEmployeeIdsByManagerAsync(
        int managerId);

    Task<List<EmployeeProject>> GetAllAsync();
}