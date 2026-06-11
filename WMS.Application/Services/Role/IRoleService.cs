using WMS.Application.DTOs.Role;

namespace WMS.Application.Services;

public interface IRoleService
{
    Task<List<RoleResponseDto>>
        GetAllAsync();

    Task<RoleResponseDto>
        GetByIdAsync(
            int roleId);

    Task<RoleResponseDto>
        CreateAsync(
            CreateRoleDto dto);

    Task UpdateAsync(
        int roleId,
        UpdateRoleDto dto);

    Task DeleteAsync(
        int roleId);
}