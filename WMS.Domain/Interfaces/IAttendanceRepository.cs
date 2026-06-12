using WMS.Domain.Entities;

namespace WMS.Domain.Interfaces;

public interface IAttendanceRepository
{
    Task AddAsync(
        Attendance attendance);

    Task UpdateAsync(
        Attendance attendance);

    Task<Attendance?> GetTodayAttendanceAsync(
        int employeeId);

    Task<List<Attendance>>
        GetByEmployeeIdAsync(
            int employeeId);
    Task<List<Attendance>>
    GetMonthlyAttendanceAsync(
        int employeeId,
        int year,
        int month);

        Task<int> GetTodayAttendanceCountAsync();

Task<int> GetMonthlyAttendanceCountAsync(
    int employeeId,
    int year,
    int month);

    Task<List<Attendance>> GetAllAsync();
}