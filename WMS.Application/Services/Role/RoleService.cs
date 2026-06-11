using WMS.Application.DTOs.Role;
using WMS.Domain.Entities;
using WMS.Domain.Interfaces;

namespace WMS.Application.Services;

public class RoleService
    : IRoleService
{
    private readonly IRoleRepository
        _roleRepository;

    public RoleService(
        IRoleRepository roleRepository)
    {
        _roleRepository =
            roleRepository;
    }

    public async Task<
        List<RoleResponseDto>>
        GetAllAsync()
    {
        var roles =
            await _roleRepository
                .GetAllAsync();

        return roles
            .Select(Map)
            .ToList();
    }

    public async Task<RoleResponseDto>
        GetByIdAsync(
            int roleId)
    {
        var role =
            await _roleRepository
                .GetByIdAsync(roleId);

        if (role == null)
        {
            throw new Exception(
                "Role not found.");
        }

        return Map(role);
    }

    public async Task<RoleResponseDto>
        CreateAsync(
            CreateRoleDto dto)
    {
        var role =
            new Role
            {
                RoleName =
                    dto.RoleName,

                Description =
                    dto.Description
            };

        await _roleRepository
            .AddAsync(role);

        return Map(role);
    }

    public async Task UpdateAsync(
        int roleId,
        UpdateRoleDto dto)
    {
        var role =
            await _roleRepository
                .GetByIdAsync(roleId);

        if (role == null)
        {
            throw new Exception(
                "Role not found.");
        }

        role.RoleName =
            dto.RoleName;

        role.Description =
            dto.Description;

        await _roleRepository
            .UpdateAsync(role);
    }

    public async Task DeleteAsync(
        int roleId)
    {
        var role =
            await _roleRepository
                .GetByIdAsync(roleId);

        if (role == null)
        {
            throw new Exception(
                "Role not found.");
        }

        if (role.Employees.Any())
        {
            throw new Exception(
                "Cannot delete role assigned to employees.");
        }

        await _roleRepository
            .DeleteAsync(role);
    }

    private static RoleResponseDto
        Map(Role role)
    {
        return new RoleResponseDto
        {
            RoleId =
                role.RoleId,

            RoleName =
                role.RoleName,

            Description =
                role.Description
        };
    }
}