namespace WMS.Application.Services.JWT;

public interface IJwtService
{
    string GenerateToken(
    int userId,
    int? employeeId,
    string username,
    string role);
}