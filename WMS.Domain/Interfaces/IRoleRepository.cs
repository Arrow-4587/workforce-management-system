namespace WMS.Domain.Interfaces;

public interface IRoleRepository
{
    Task<bool> ExistsAsync(int roleId);
}
