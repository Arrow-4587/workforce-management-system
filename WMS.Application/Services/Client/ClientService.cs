using WMS.Application.DTOs.Client;
using WMS.Domain.Interfaces;
using ClientEntity = WMS.Domain.Entities.Client;

namespace WMS.Application.Services.Client;

public class ClientService : IClientService
{
    private readonly IClientRepository
        _clientRepository;

    public ClientService(
        IClientRepository clientRepository)
    {
        _clientRepository =
            clientRepository;
    }

    public async Task<List<ClientResponseDto>>
        GetAllAsync()
    {
        var clients =
            await _clientRepository
                .GetAllAsync();

        return clients
            .Select(Map)
            .ToList();
    }

    public async Task<ClientResponseDto>
        GetByIdAsync(
            int clientId)
    {
        var client =
            await _clientRepository
                .GetByIdAsync(clientId);

        if (client == null)
        {
            throw new Exception(
                "Client not found.");
        }

        return Map(client);
    }

    public async Task<ClientResponseDto>
        CreateAsync(
            CreateClientDto dto)
    {
        var client =
            new ClientEntity
            {
                ClientName =
                    dto.ClientName,

                ClientAddress =
                    dto.ClientAddress,

                ClientPhoneNumber =
                    dto.ClientPhoneNumber,

                ClientLocation =
                    dto.ClientLocation,

                Status = true
            };

        await _clientRepository
            .AddAsync(client);

        return Map(client);
    }

    public async Task<ClientResponseDto>
        UpdateAsync(
            int clientId,
            UpdateClientDto dto)
    {
        var client =
            await _clientRepository
                .GetByIdAsync(clientId);

        if (client == null)
        {
            throw new Exception(
                "Client not found.");
        }

        client.ClientName =
            dto.ClientName;

        client.ClientAddress =
            dto.ClientAddress;

        client.ClientPhoneNumber =
            dto.ClientPhoneNumber;

        client.ClientLocation =
            dto.ClientLocation;

        client.Status =
            dto.Status;

        await _clientRepository
            .UpdateAsync(client);

        return Map(client);
    }

    public async Task DeleteAsync(
        int clientId)
    {
        var client =
            await _clientRepository
                .GetByIdAsync(clientId);

        if (client == null)
        {
            throw new Exception(
                "Client not found.");
        }

        await _clientRepository
            .DeleteAsync(client);
    }

    private static ClientResponseDto
        Map(
            ClientEntity client)
    {
        return new ClientResponseDto
        {
            ClientId =
                client.ClientId,

            ClientName =
                client.ClientName,

            ClientAddress =
                client.ClientAddress,

            ClientPhoneNumber =
                client.ClientPhoneNumber,

            ClientLocation =
                client.ClientLocation,

            Status =
                client.Status
        };
    }
}