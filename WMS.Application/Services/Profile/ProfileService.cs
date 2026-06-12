
using BCrypt.Net;
using WMS.Application.DTOs.Profile;
using WMS.Domain.Interfaces;
using WMS.Application.DTOs.Auth;

namespace WMS.Application.Services.Profile;

public class ProfileService
    : IProfileService
{
    private readonly IEmployeeRepository
        _employeeRepository;

    private readonly IAuthRepository
        _authRepository;

    public ProfileService(
        IEmployeeRepository employeeRepository,
        IAuthRepository authRepository)
    {
        _employeeRepository =
            employeeRepository;

        _authRepository =
            authRepository;
    }

    public async Task<ProfileResponseDto>
        GetProfileAsync(
            int userId)
    {
        var employee =
            await _employeeRepository
                .GetByUserIdAsync(userId);

        if (employee == null)
        {
            var user = await _authRepository.GetByUserIdAsync(userId);
            if (user == null)
            {
                throw new Exception("User not found.");
            }

            return new ProfileResponseDto
            {
                EmployeeId = 0,
                FirstName = "System",
                LastName = "Administrator",
                Email = "admin@system.local",
                PhoneNumber = "N/A",
                Gender = 'O',
                DOB = DateTime.UtcNow.Date,
                DOJ = DateTime.UtcNow.Date,
                Department = "System Administration",
                Role = user.Role?.RoleName ?? "Admin",
                Username = user.Username
            };
        }

        return new ProfileResponseDto
        {
            EmployeeId =
                employee.EmployeeId,

            FirstName =
                employee.FirstName,

            LastName =
                employee.LastName,

            Email =
                employee.Email,

            PhoneNumber =
                employee.PhoneNumber,

            Gender =
                employee.Gender,

            DOB =
                employee.DOB,

            DOJ =
                employee.DOJ,

            Department =
                employee.Department?.DepartmentName
                ?? string.Empty,

            Role =
                employee.Role?.RoleName
                ?? string.Empty,

            Username =
                employee.UserLogin?.Username
                ?? string.Empty
        };
    }

    public async Task UpdateProfileAsync(
        int userId,
        UpdateProfileDto dto)
    {
        var employee =
            await _employeeRepository
                .GetByUserIdAsync(userId);

        if (employee == null)
        {
            throw new Exception(
                "The system administrator profile cannot be edited.");
        }

        employee.FirstName =
            dto.FirstName;

        employee.LastName =
            dto.LastName;

        employee.Email =
            dto.Email;

        employee.PhoneNumber =
            dto.PhoneNumber;

        employee.UpdatedOn =
            DateTime.UtcNow;

        await _employeeRepository
            .UpdateAsync(employee);
    }

    public async Task ChangePasswordAsync(
        int userId,
        ChangePasswordDto dto)
    {
        var user =
            await _authRepository
                .GetByUserIdAsync(userId);

        if (user == null)
        {
            throw new Exception(
                "User not found.");
        }

        bool valid =
            BCrypt.Net.BCrypt.Verify(
                dto.CurrentPassword,
                user.PasswordHash);

        if (!valid)
        {
            throw new Exception(
                "Current password is incorrect.");
        }

        user.PasswordHash =
            BCrypt.Net.BCrypt.HashPassword(
                dto.NewPassword);

        await _authRepository
            .UpdateAsync(user);
    }
}