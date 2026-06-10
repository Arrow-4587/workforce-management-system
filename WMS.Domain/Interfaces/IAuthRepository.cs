using WMS.Domain.Entities;

namespace WMS.Domain.Interfaces;

public interface IAuthRepository
{
    Task<UserLogin?> GetByUsernameAsync(string username);
}