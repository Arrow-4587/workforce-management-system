using System.ComponentModel.DataAnnotations;

namespace WMS.Application.DTOs.Client;

public class UpdateClientDto
{
    [Required]
    public string ClientName { get; set; }
        = string.Empty;

    public string? ClientAddress { get; set; }

    public string? ClientPhoneNumber { get; set; }

    public string? ClientLocation { get; set; }

    public bool Status { get; set; }
}