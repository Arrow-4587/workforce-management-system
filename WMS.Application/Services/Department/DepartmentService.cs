    using WMS.Application.DTOs.Department;
    using WMS.Domain.Interfaces;
    using DepartmentEntity = WMS.Domain.Entities.Department;

    namespace WMS.Application.Services.Department;

    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository
            _departmentRepository;
        private readonly IEmployeeRepository
        _employeeRepository;

        public DepartmentService(
          IDepartmentRepository departmentRepository,
          IEmployeeRepository employeeRepository)
        {
            _departmentRepository =
                departmentRepository;

            _employeeRepository =
                employeeRepository;
        }

        private static DepartmentResponseDto Map(
            DepartmentEntity department)
        {
            return new DepartmentResponseDto
            {
                DepartmentId = department.DepartmentId,
                DepartmentName = department.DepartmentName,
                Description = department.Description,
                CreatedOn = department.CreatedOn
            };
        }

        public async Task<List<DepartmentResponseDto>>
            GetAllAsync()
        {
            var departments =
                await _departmentRepository
                    .GetAllAsync();

            return departments
                .Select(Map)
                .ToList();
        }

        public async Task<DepartmentResponseDto?>
            GetByIdAsync(int departmentId)
        {
            var department =
                await _departmentRepository
                    .GetByIdAsync(departmentId);

            return department == null
                ? null
                : Map(department);
        }

        public async Task<DepartmentResponseDto>
            CreateAsync(CreateDepartmentDto dto)
        {
            var existingDepartment =
                await _departmentRepository
                    .GetByNameAsync(
                        dto.DepartmentName);

            if (existingDepartment != null)
            {
                throw new Exception(
                    "Department already exists.");
            }

            var department =
                new DepartmentEntity
                {
                    DepartmentName =
                        dto.DepartmentName,

                    Description =
                        dto.Description,

                    CreatedOn =
                        DateTime.UtcNow
                };

            await _departmentRepository
                .AddAsync(department);

            return Map(department);
        }

        public async Task UpdateAsync(
            int departmentId,
            UpdateDepartmentDto dto)
        {
            var department =
                await _departmentRepository
                    .GetByIdAsync(departmentId);

            if (department == null)
            {
                throw new Exception(
                    "Department not found.");
            }
            var existingDepartment =
        await _departmentRepository
            .GetByNameAsync(
                dto.DepartmentName);

            if (existingDepartment != null &&
                existingDepartment.DepartmentId != departmentId)
            {
                throw new Exception(
                    "Department already exists.");
            }

            department.DepartmentName =
                dto.DepartmentName;

            department.Description =
                dto.Description;

            await _departmentRepository
                .UpdateAsync(department);
        }

        public async Task DeleteAsync(
            int departmentId)
        {
            var department =
                await _departmentRepository
                    .GetByIdAsync(departmentId);

            if (department == null)
            {
                throw new Exception(
                    "Department not found.");
            }
            var hasEmployees =
        await _employeeRepository
            .DepartmentHasEmployeesAsync(
                departmentId);

            if (hasEmployees)
            {
                throw new Exception(
                    "Department contains employees and cannot be deleted.");
            }

            await _departmentRepository
                .DeleteAsync(department);
        }
        public async Task<List<DepartmentResponseDto>>
        SearchByNameAsync(string name)
        {
            var departments =
                await _departmentRepository
                    .SearchByNameAsync(name);

            return departments
                .Select(Map)
                .ToList();
        }
    }