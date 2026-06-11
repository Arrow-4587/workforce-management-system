using WMS.Application.DTOs.Project;
using WMS.Domain.Interfaces;
using ProjectEntity =
    WMS.Domain.Entities.Project;

namespace WMS.Application.Services.Project;

public class ProjectService
    : IProjectService
{
    private readonly IProjectRepository
        _projectRepository;

    private readonly IClientRepository
        _clientRepository;

    private readonly IEmployeeRepository
        _employeeRepository;

    public ProjectService(
        IProjectRepository projectRepository,
        IClientRepository clientRepository,
        IEmployeeRepository employeeRepository)
    {
        _projectRepository =
            projectRepository;

        _clientRepository =
            clientRepository;

        _employeeRepository =
            employeeRepository;
    }

    public async Task<List<ProjectResponseDto>>
        GetAllAsync()
    {
        var projects =
            await _projectRepository
                .GetAllAsync();

        return projects
            .Select(Map)
            .ToList();
    }

    public async Task<ProjectResponseDto>
        GetByIdAsync(
            int projectId)
    {
        var project =
            await _projectRepository
                .GetByIdAsync(projectId);

        if (project == null)
        {
            throw new Exception(
                "Project not found.");
        }

        return Map(project);
    }

    public async Task<ProjectResponseDto>
        CreateAsync(
            CreateProjectDto dto)
    {
        if (!await _clientRepository
            .ExistsAsync(dto.ClientId))
        {
            throw new Exception(
                "Client does not exist.");
        }

        //if (!await _employeeRepository
        //    .ExistsAsync(dto.ManagerId))
        //{
        //    throw new Exception(
        //        "Manager does not exist.");
        //}
        if (!await _employeeRepository
    .IsManagerAsync(dto.ManagerId))
        {
            throw new Exception(
                "Selected employee is not a manager.");
        }

        var project =
            new ProjectEntity
            {
                ProjectName =
                    dto.ProjectName,

                ClientId =
                    dto.ClientId,

                ManagerId =
                    dto.ManagerId,

                StartDate =
                    dto.StartDate,

                EndDate =
                    dto.EndDate,

                Status = "Active"
            };

        await _projectRepository
            .AddAsync(project);

        project =
            await _projectRepository
                .GetByIdAsync(
                    project.ProjectId)
            ?? project;

        return Map(project);
    }

    public async Task<ProjectResponseDto>
        UpdateAsync(
            int projectId,
            UpdateProjectDto dto)
    {
        var project =
            await _projectRepository
                .GetByIdAsync(projectId);

        if (project == null)
        {
            throw new Exception(
                "Project not found.");
        }
        if (!await _employeeRepository
    .IsManagerAsync(dto.ManagerId))
        {
            throw new Exception(
                "Selected employee is not a manager.");
        }

        project.ProjectName =
            dto.ProjectName;

        project.ClientId =
            dto.ClientId;

        project.ManagerId =
            dto.ManagerId;

        project.StartDate =
            dto.StartDate;

        project.EndDate =
            dto.EndDate;

        project.Status =
            dto.Status;

        await _projectRepository
            .UpdateAsync(project);

        return Map(project);
    }

    public async Task DeleteAsync(
        int projectId)
    {
        var project =
            await _projectRepository
                .GetByIdAsync(projectId);

        if (project == null)
        {
            throw new Exception(
                "Project not found.");
        }

        await _projectRepository
            .DeleteAsync(project);
    }

    public async Task<List<ProjectResponseDto>>
    GetMyProjectsAsync(
        int managerId)
    {
        var projects =
            await _projectRepository
                .GetByManagerIdAsync(
                    managerId);

        return projects
            .Select(Map)
            .ToList();
    }
    private static ProjectResponseDto
        Map(
            ProjectEntity project)
    {
        return new ProjectResponseDto
        {
            ProjectId =
                project.ProjectId,

            ProjectName =
                project.ProjectName,

            ClientId =
                project.ClientId,

            ClientName =
                project.Client?.ClientName
                ?? string.Empty,

            ManagerId =
                project.ManagerId,

            ManagerName =
                project.Manager == null
                ? string.Empty
                : $"{project.Manager.FirstName} {project.Manager.LastName}",

            StartDate =
                project.StartDate,

            EndDate =
                project.EndDate,

            Status =
                project.Status
        };
    }
}