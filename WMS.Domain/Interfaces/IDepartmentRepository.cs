using WMS.Domain.Entities;

namespace WMS.Domain.Interfaces;

public interface IDepartmentRepository
{
    Task<List<Department>> GetAllAsync();

    Task<Department?> GetByIdAsync(int departmentId);

    Task<Department?> GetByNameAsync(string departmentName);

    Task AddAsync(Department department);

    Task UpdateAsync(Department department);

    Task DeleteAsync(Department department);

    Task<bool> ExistsAsync(int departmentId);
    Task<List<Department>>
    SearchByNameAsync(string name);
}