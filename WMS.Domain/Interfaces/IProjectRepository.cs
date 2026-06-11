using WMS.Domain.Entities;

namespace WMS.Domain.Interfaces;

public interface IProjectRepository
{
    Task<List<Project>> GetAllAsync();

    Task<Project?> GetByIdAsync(
        int projectId);

    Task AddAsync(
        Project project);

    Task UpdateAsync(
        Project project);

    Task DeleteAsync(
        Project project);

    Task<bool> ExistsAsync(
        int projectId);
    Task<List<Project>>
    GetByManagerIdAsync(
        int managerId);
        Task<int> GetTotalCountAsync();
}