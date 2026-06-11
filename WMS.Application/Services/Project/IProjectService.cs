using WMS.Application.DTOs.Project;

namespace WMS.Application.Services.Project;

public interface IProjectService
{
    Task<List<ProjectResponseDto>>
        GetAllAsync();

    Task<ProjectResponseDto>
        GetByIdAsync(
            int projectId);

    Task<ProjectResponseDto>
        CreateAsync(
            CreateProjectDto dto);

    Task<ProjectResponseDto>
        UpdateAsync(
            int projectId,
            UpdateProjectDto dto);

    Task DeleteAsync(
        int projectId);
    Task<List<ProjectResponseDto>>
    GetMyProjectsAsync(
        int managerId);
}