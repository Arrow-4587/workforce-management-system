using Moq;
using WMS.Application.DTOs.Auth;
using WMS.Application.Services.Auth;
using WMS.Application.Services.JWT;
using WMS.Domain.Entities;
using WMS.Domain.Interfaces;
using Xunit;

namespace WMS.Tests.Services.Auth;

public class AuthServiceTests
{
    private readonly Mock<IAuthRepository>
        _authRepositoryMock;

    private readonly Mock<IJwtService>
        _jwtServiceMock;

    private readonly AuthService
        _authService;

    public AuthServiceTests()
    {
        _authRepositoryMock =
            new Mock<IAuthRepository>();

        _jwtServiceMock =
            new Mock<IJwtService>();

        _authService =
            new AuthService(
                _authRepositoryMock.Object,
                _jwtServiceMock.Object);
    }

    [Fact]
    public async Task LoginAsync_UserNotFound_ReturnsNull()
    {
        _authRepositoryMock
            .Setup(x =>
                x.GetByUsernameAsync("admin"))
            .ReturnsAsync(
                (UserLogin?)null);

        var result =
            await _authService
                .LoginAsync(
                    new LoginRequestDto
                    {
                        Username = "admin",
                        Password = "Password123!"
                    });

        Assert.Null(result);
    }

    [Fact]
    public async Task LoginAsync_InvalidPassword_ReturnsNull()
    {
        var user = new UserLogin
        {
            UserId = 1,
            Username = "admin",
            PasswordHash =
                BCrypt.Net.BCrypt.HashPassword(
                    "CorrectPassword123!")
        };

        _authRepositoryMock
            .Setup(x =>
                x.GetByUsernameAsync("admin"))
            .ReturnsAsync(user);

        var result =
            await _authService
                .LoginAsync(
                    new LoginRequestDto
                    {
                        Username = "admin",
                        Password = "WrongPassword"
                    });

        Assert.Null(result);
    }

    [Fact]
    public async Task LoginAsync_ValidCredentials_ReturnsToken()
    {
        var user = new UserLogin
        {
            UserId = 1,
            EmployeeId = 10,
            Username = "admin",
            PasswordHash =
                BCrypt.Net.BCrypt.HashPassword(
                    "Password123!"),
            Role = new Role
            {
                RoleName = "Admin"
            }
        };

        _authRepositoryMock
            .Setup(x =>
                x.GetByUsernameAsync("admin"))
            .ReturnsAsync(user);

        _jwtServiceMock
            .Setup(x =>
                x.GenerateToken(
                    user.UserId,
                    user.EmployeeId,
                    user.Username,
                    "Admin"))
            .Returns("mock-jwt-token");

        var result =
            await _authService
                .LoginAsync(
                    new LoginRequestDto
                    {
                        Username = "admin",
                        Password = "Password123!"
                    });

        Assert.NotNull(result);

        Assert.Equal(
            "mock-jwt-token",
            result.Token);

        _authRepositoryMock.Verify(
            x => x.UpdateAsync(
                It.IsAny<UserLogin>()),
            Times.Once);
    }

    [Fact]
    public async Task ChangePasswordAsync_UserNotFound_ThrowsException()
    {
        _authRepositoryMock
            .Setup(x =>
                x.GetByUserIdAsync(1))
            .ReturnsAsync(
                (UserLogin?)null);

        var dto =
            new ChangePasswordDto
            {
                CurrentPassword =
                    "OldPassword123!",
                NewPassword =
                    "NewPassword123!"
            };

        var ex =
            await Assert.ThrowsAsync<Exception>(
                () =>
                    _authService
                        .ChangePasswordAsync(
                            1,
                            dto));

        Assert.Equal(
            "User not found.",
            ex.Message);
    }

    [Fact]
    public async Task ChangePasswordAsync_InvalidCurrentPassword_ThrowsException()
    {
        var user = new UserLogin
        {
            UserId = 1,
            PasswordHash =
                BCrypt.Net.BCrypt.HashPassword(
                    "CorrectPassword123!")
        };

        _authRepositoryMock
            .Setup(x =>
                x.GetByUserIdAsync(1))
            .ReturnsAsync(user);

        var dto =
            new ChangePasswordDto
            {
                CurrentPassword =
                    "WrongPassword",
                NewPassword =
                    "NewPassword123!"
            };

        var ex =
            await Assert.ThrowsAsync<Exception>(
                () =>
                    _authService
                        .ChangePasswordAsync(
                            1,
                            dto));

        Assert.Equal(
            "Current password is incorrect.",
            ex.Message);
    }

    [Fact]
    public async Task ChangePasswordAsync_SamePassword_ThrowsException()
    {
        var user = new UserLogin
        {
            UserId = 1,
            PasswordHash =
                BCrypt.Net.BCrypt.HashPassword(
                    "Password123!")
        };

        _authRepositoryMock
            .Setup(x =>
                x.GetByUserIdAsync(1))
            .ReturnsAsync(user);

        var dto =
            new ChangePasswordDto
            {
                CurrentPassword =
                    "Password123!",
                NewPassword =
                    "Password123!"
            };

        var ex =
            await Assert.ThrowsAsync<Exception>(
                () =>
                    _authService
                        .ChangePasswordAsync(
                            1,
                            dto));

        Assert.Equal(
            "New password must be different from current password.",
            ex.Message);
    }

    [Fact]
    public async Task ChangePasswordAsync_ValidPassword_UpdatesUser()
    {
        var user = new UserLogin
        {
            UserId = 1,
            PasswordHash =
                BCrypt.Net.BCrypt.HashPassword(
                    "OldPassword123!"),
            IsFirstLogin = true
        };

        _authRepositoryMock
            .Setup(x =>
                x.GetByUserIdAsync(1))
            .ReturnsAsync(user);

        var dto =
            new ChangePasswordDto
            {
                CurrentPassword =
                    "OldPassword123!",
                NewPassword =
                    "NewPassword123!"
            };

        await _authService
            .ChangePasswordAsync(
                1,
                dto);

        Assert.False(
            user.IsFirstLogin);

        _authRepositoryMock.Verify(
            x => x.UpdateAsync(
                It.IsAny<UserLogin>()),
            Times.Once);
    }
}