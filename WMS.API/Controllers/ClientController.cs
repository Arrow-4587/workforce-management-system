using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WMS.Application.DTOs.Client;
using WMS.Application.Services.Client;

namespace WMS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]
public class ClientController : ControllerBase
{
    private readonly IClientService
        _clientService;

    public ClientController(
        IClientService clientService)
    {
        _clientService =
            clientService;
    }

    [HttpGet]
    public async Task<IActionResult>
        GetAll()
    {
        return Ok(
            await _clientService
                .GetAllAsync());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult>
        GetById(
            int id)
    {
        return Ok(
            await _clientService
                .GetByIdAsync(id));
    }

    [HttpPost]
    public async Task<IActionResult>
        Create(
            CreateClientDto dto)
    {
        return Ok(
            await _clientService
                .CreateAsync(dto));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult>
        Update(
            int id,
            UpdateClientDto dto)
    {
        return Ok(
            await _clientService
                .UpdateAsync(
                    id,
                    dto));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult>
        Delete(
            int id)
    {
        await _clientService
            .DeleteAsync(id);

        return Ok(
            "Client deleted successfully.");
    }
}