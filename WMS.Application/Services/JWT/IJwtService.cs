namespace WMS.Application.Services.JWT;

public interface IJwtService
{
    string GenerateToken(int userId, string username, string role);
}