using WMS.Domain.Entities;

namespace WMS.Domain.Interfaces;

public interface IAuthRepository
{
    Task<UserLogin?> GetByUsernameAsync(
        string username);

    Task<bool> UsernameExistsAsync(
        string username);

    Task AddAsync(
        UserLogin userLogin);
    Task<UserLogin?> GetByUserIdAsync(
    int userId);

    Task UpdateAsync(
        UserLogin userLogin);
}