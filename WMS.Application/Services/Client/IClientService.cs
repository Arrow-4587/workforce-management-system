using WMS.Application.DTOs.Client;

namespace WMS.Application.Services.Client;

public interface IClientService
{
    Task<List<ClientResponseDto>>
        GetAllAsync();

    Task<ClientResponseDto>
        GetByIdAsync(
            int clientId);

    Task<ClientResponseDto>
        CreateAsync(
            CreateClientDto dto);

    Task<ClientResponseDto>
        UpdateAsync(
            int clientId,
            UpdateClientDto dto);

    Task DeleteAsync(
        int clientId);
}