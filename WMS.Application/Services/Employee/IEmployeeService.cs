using WMS.Application.DTOs.Employee;

namespace WMS.Application.Services.Employee;

public interface IEmployeeService
{
    Task<List<EmployeeResponseDto>> GetAllAsync();

    Task<EmployeeResponseDto?> GetByIdAsync(int employeeId);

    Task<EmployeeResponseDto> CreateAsync(CreateEmployeeDto dto);

    Task UpdateAsync(int employeeId, UpdateEmployeeDto dto);

    Task DeleteAsync(int employeeId);

    Task<List<EmployeeResponseDto>> SearchByNameAsync(string name);

    Task<List<EmployeeResponseDto>> GetByDepartmentAsync(int departmentId);

    Task<List<EmployeeResponseDto>> GetByRoleAsync(int roleId);
    Task<EmployeeResponseDto?>
    GetMyProfileAsync(int userId);
}