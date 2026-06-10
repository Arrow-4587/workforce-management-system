using WMS.Domain.Entities;

namespace WMS.Domain.Interfaces;

public interface IDepartmentRepository
{
    Task<bool> ExistsAsync(int departmentId);
}
