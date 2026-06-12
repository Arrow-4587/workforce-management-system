using WMS.Application.DTOs.Auth;

namespace WMS.Application.DTOs.Profile;

public class UpdateProfileDto
{
    public string FirstName { get; set; }
        = string.Empty;

    public string LastName { get; set; }
        = string.Empty;

    public string Email { get; set; }
        = string.Empty;

    public string PhoneNumber { get; set; }
        = string.Empty;
}