using WMS.Application.DTOs.Employee;
using WMS.Domain.Interfaces;
using EmployeeEntity = WMS.Domain.Entities.Employee;

namespace WMS.Application.Services.Employee;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IRoleRepository _roleRepository;

    public EmployeeService(
        IEmployeeRepository employeeRepository,
        IDepartmentRepository departmentRepository,
        IRoleRepository roleRepository)
    {
        _employeeRepository = employeeRepository;
        _departmentRepository = departmentRepository;
        _roleRepository = roleRepository;
    }

    private static EmployeeResponseDto Map(EmployeeEntity employee)
    {
        return new EmployeeResponseDto
        {
            EmployeeId = employee.EmployeeId,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Email = employee.Email,
            PhoneNumber = employee.PhoneNumber,
            DepartmentName = employee.Department?.DepartmentName ?? string.Empty,
            RoleName = employee.Role?.RoleName ?? string.Empty,
            Status = employee.Status
        };
    }

    public async Task<List<EmployeeResponseDto>> GetAllAsync()
    {
        var employees = await _employeeRepository.GetAllAsync();

        return employees
            .Select(Map)
            .ToList();
    }

    public async Task<EmployeeResponseDto?> GetByIdAsync(int employeeId)
    {
        var employee =
            await _employeeRepository.GetByIdAsync(employeeId);

        return employee == null
            ? null
            : Map(employee);
    }

    public async Task<EmployeeResponseDto> CreateAsync(
        CreateEmployeeDto dto)
    {
        var existingEmployee =
            await _employeeRepository
                .GetByEmailAsync(dto.Email);

        if (existingEmployee != null)
            throw new Exception("Email already exists.");
        if (!await _departmentRepository
    .ExistsAsync(dto.DepartmentId))
        {
            throw new Exception(
                "Department does not exist.");
        }

        if (!await _roleRepository
            .ExistsAsync(dto.RoleId))
        {
            throw new Exception(
                "Role does not exist.");
        }

        int age = DateTime.Today.Year - dto.DOB.Year;

        if (dto.DOB.Date > DateTime.Today.AddYears(-age))
            age--;

        if (dto.DOJ < dto.DOB)
        {
            throw new Exception(
                "Date of Joining cannot be earlier than Date of Birth.");
        }
        if (age < 18)
            throw new Exception(
                "Employee must be at least 18 years old.");
        if (dto.Gender != 'M' &&
    dto.Gender != 'F' &&
    dto.Gender != 'O')
        {
            throw new Exception(
                "Gender must be M, F or O.");
        }

        var employee = new EmployeeEntity
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            Gender = dto.Gender,
            DOB = dto.DOB,
            DOJ = dto.DOJ,
            DepartmentId = dto.DepartmentId,
            RoleId = dto.RoleId,
            Status = "Active",
            CreatedOn = DateTime.UtcNow
        };

        await _employeeRepository.AddAsync(employee);

        return Map(employee);
    }

    public async Task UpdateAsync(
        int employeeId,
        UpdateEmployeeDto dto)
    {
        var emailExists =
await _employeeRepository
 .EmailExistsAsync(
     dto.Email,
     employeeId);

        if (emailExists)
        {
            throw new Exception(
                "Email already exists.");
        }
        if (!await _departmentRepository
    .ExistsAsync(dto.DepartmentId))
        {
            throw new Exception(
                "Department does not exist.");
        }

        if (!await _roleRepository
            .ExistsAsync(dto.RoleId))
        {
            throw new Exception(
                "Role does not exist.");
        }
        var employee =
            await _employeeRepository
                .GetByIdAsync(employeeId);

        if (employee == null)
            throw new Exception("Employee not found.");
        if (dto.DOJ < dto.DOB)
        {
            throw new Exception(
                "Date of Joining cannot be earlier than Date of Birth.");
        }
        if (dto.Gender != 'M' &&
    dto.Gender != 'F' &&
    dto.Gender != 'O')
        {
            throw new Exception(
                "Gender must be M, F or O.");
        }

        if (dto.Status != "Active" &&
    dto.Status != "Inactive")
        {
            throw new Exception(
                "Status must be Active or Inactive.");
        }

        employee.FirstName = dto.FirstName;
        employee.LastName = dto.LastName;
        employee.Email = dto.Email;
        employee.PhoneNumber = dto.PhoneNumber;
        employee.Gender = dto.Gender;
        employee.DOJ = dto.DOJ;
        employee.DepartmentId = dto.DepartmentId;
        employee.RoleId = dto.RoleId;
        employee.Status = dto.Status;
        employee.UpdatedOn = DateTime.UtcNow;

        await _employeeRepository.UpdateAsync(employee);

 
    }

    public async Task DeleteAsync(int employeeId)
    {
        var employee =
            await _employeeRepository
                .GetByIdAsync(employeeId);

        if (employee == null)
            throw new Exception("Employee not found.");

        await _employeeRepository.DeleteAsync(employee);
    }

    public async Task<List<EmployeeResponseDto>> SearchByNameAsync(
        string name)
    {
        var employees =
            await _employeeRepository.SearchByNameAsync(name);

        return employees
            .Select(Map)
            .ToList();
    }

    public async Task<List<EmployeeResponseDto>> GetByDepartmentAsync(
        int departmentId)
    {
        var employees =
            await _employeeRepository
                .GetByDepartmentAsync(departmentId);

        return employees
            .Select(Map)
            .ToList();
    }

    public async Task<List<EmployeeResponseDto>> GetByRoleAsync(
        int roleId)
    {
        var employees =
            await _employeeRepository
                .GetByRoleAsync(roleId);

        return employees
            .Select(Map)
            .ToList();
    }
}