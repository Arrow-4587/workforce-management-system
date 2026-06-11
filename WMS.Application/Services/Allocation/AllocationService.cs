using WMS.Application.DTOs.Allocation;
using WMS.Domain.Interfaces;
using EmployeeProjectEntity =
    WMS.Domain.Entities.EmployeeProject;

namespace WMS.Application.Services.Allocation;

public class AllocationService
    : IAllocationService
{
    private readonly IEmployeeProjectRepository
        _allocationRepository;

    private readonly IEmployeeRepository
        _employeeRepository;

    private readonly IProjectRepository
        _projectRepository;

    public AllocationService(
        IEmployeeProjectRepository allocationRepository,
        IEmployeeRepository employeeRepository,
        IProjectRepository projectRepository)
    {
        _allocationRepository =
            allocationRepository;

        _employeeRepository =
            employeeRepository;

        _projectRepository =
            projectRepository;
    }

    public async Task<AllocationResponseDto>
        AllocateAsync(
            AllocateEmployeeDto dto)
    {
        if (!await _employeeRepository
            .ExistsAsync(dto.EmployeeId))
        {
            throw new Exception(
                "Employee does not exist.");
        }

        if (!await _projectRepository
            .ExistsAsync(dto.ProjectId))
        {
            throw new Exception(
                "Project does not exist.");
        }

        var existing =
            await _allocationRepository
                .GetActiveAllocationAsync(
                    dto.EmployeeId,
                    dto.ProjectId);

        if (existing != null)
        {
            throw new Exception(
                "Employee is already allocated to this project.");
        }

        var allocation =
            new EmployeeProjectEntity
            {
                EmployeeId =
                    dto.EmployeeId,

                ProjectId =
                    dto.ProjectId,

                AllocatedOn =
                    DateTime.UtcNow
            };

        await _allocationRepository
            .AddAsync(allocation);

        allocation =
            await _allocationRepository
                .GetByIdAsync(
                    allocation.AllocationId)
            ?? allocation;

        return Map(allocation);
    }

    public async Task ReleaseAsync(
        int allocationId)
    {
        var allocation =
            await _allocationRepository
                .GetByIdAsync(
                    allocationId);

        if (allocation == null)
        {
            throw new Exception(
                "Allocation not found.");
        }

        allocation.ReleasedOn =
            DateTime.UtcNow;

        await _allocationRepository
            .UpdateAsync(allocation);
    }

    public async Task<List<AllocationResponseDto>>
        GetByProjectAsync(
            int projectId)
    {
        var allocations =
            await _allocationRepository
                .GetByProjectIdAsync(
                    projectId);

        return allocations
            .Select(Map)
            .ToList();
    }

    public async Task<List<AllocationResponseDto>>
        GetByEmployeeAsync(
            int employeeId)
    {
        var allocations =
            await _allocationRepository
                .GetByEmployeeIdAsync(
                    employeeId);

        return allocations
            .Select(Map)
            .ToList();
    }
    public async Task<List<AllocationResponseDto>>
    GetMyProjectsAsync(
        int employeeId)
    {
        var allocations =
            await _allocationRepository
                .GetByEmployeeIdAsync(
                    employeeId);

        return allocations
            .Select(Map)
            .ToList();
    }

    private static AllocationResponseDto
        Map(
            EmployeeProjectEntity allocation)
    {
        return new AllocationResponseDto
        {
            AllocationId =
                allocation.AllocationId,

            EmployeeId =
                allocation.EmployeeId,

            EmployeeName =
                allocation.Employee == null
                ? string.Empty
                : $"{allocation.Employee.FirstName} {allocation.Employee.LastName}",

            ProjectId =
                allocation.ProjectId,

            ProjectName =
                allocation.Project?.ProjectName
                ?? string.Empty,

            AllocatedOn =
                allocation.AllocatedOn,

            ReleasedOn =
                allocation.ReleasedOn
        };
    }
}