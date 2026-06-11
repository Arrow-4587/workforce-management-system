using WMS.Application.DTOs.Dashboard;

namespace WMS.Application.Services.Dashboard;

public interface IDashboardService
{
    Task<AdminDashboardDto>
        GetAdminDashboardAsync();

    Task<ManagerDashboardDto>
        GetManagerDashboardAsync(
            int managerId);

    Task<EmployeeDashboardDto>
        GetEmployeeDashboardAsync(
            int employeeId);
}