using WMS.Domain.Entities;

namespace WMS.Domain.Interfaces;

public interface IRoleRepository
{
    Task<bool> ExistsAsync(
        int roleId);

    Task<List<Role>> GetAllAsync();

    Task<Role?> GetByIdAsync(
        int roleId);

    Task AddAsync(
        Role role);

    Task UpdateAsync(
        Role role);

    Task DeleteAsync(
        Role role);
}