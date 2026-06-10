using WMS.Application.DTOs.Department;

namespace WMS.Application.Services.Department;

public interface IDepartmentService
{
    Task<List<DepartmentResponseDto>> GetAllAsync();

    Task<DepartmentResponseDto?> GetByIdAsync(
        int departmentId);

    Task<DepartmentResponseDto> CreateAsync(
        CreateDepartmentDto dto);

    Task UpdateAsync(
        int departmentId,
        UpdateDepartmentDto dto);

    Task DeleteAsync(
        int departmentId);
    Task<List<DepartmentResponseDto>>
    SearchByNameAsync(string name);
}