using WMS.Domain.Entities;

namespace WMS.Domain.Interfaces;

public interface IClientRepository
{
    Task<List<Client>> GetAllAsync();

    Task<Client?> GetByIdAsync(
        int clientId);

    Task AddAsync(
        Client client);

    Task UpdateAsync(
        Client client);

    Task DeleteAsync(
        Client client);

    Task<bool> ExistsAsync(
        int clientId);
}