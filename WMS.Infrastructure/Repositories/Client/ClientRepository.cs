using Microsoft.EntityFrameworkCore;
using WMS.Domain.Entities;
using WMS.Domain.Interfaces;
using WMS.Infrastructure.Data;

namespace WMS.Infrastructure.Repositories;

public class ClientRepository
    : IClientRepository
{
    private readonly WmsDbContext _context;

    public ClientRepository(
        WmsDbContext context)
    {
        _context = context;
    }

    public async Task<List<Client>>
        GetAllAsync()
    {
        return await _context.Clients
            .OrderBy(c =>
                c.ClientName)
            .ToListAsync();
    }

    public async Task<Client?>
        GetByIdAsync(
            int clientId)
    {
        return await _context.Clients
            .FirstOrDefaultAsync(c =>
                c.ClientId == clientId);
    }

    public async Task AddAsync(
        Client client)
    {
        await _context.Clients
            .AddAsync(client);

        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(
        Client client)
    {
        _context.Clients
            .Update(client);

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(
        Client client)
    {
        _context.Clients
            .Remove(client);

        await _context.SaveChangesAsync();
    }

    public async Task<bool>
        ExistsAsync(
            int clientId)
    {
        return await _context.Clients
            .AnyAsync(c =>
                c.ClientId == clientId);
    }
}