using WMS.Application.DTOs.Allocation;

namespace WMS.Application.Services.Allocation;

public interface IAllocationService
{
    Task<AllocationResponseDto>
        AllocateAsync(
            AllocateEmployeeDto dto);

    Task ReleaseAsync(
        int allocationId);

    Task<List<AllocationResponseDto>>
        GetByProjectAsync(
            int projectId);

    Task<List<AllocationResponseDto>>
        GetByEmployeeAsync(
            int employeeId);
    Task<List<AllocationResponseDto>>
    GetMyProjectsAsync(
        int employeeId);
}