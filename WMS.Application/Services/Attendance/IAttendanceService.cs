using WMS.Application.DTOs.Attendance;

namespace WMS.Application.Services.Attendance;

public interface IAttendanceService
{
    Task<AttendanceResponseDto>
        CheckInAsync(
            int employeeId,
            CheckInDto dto);

    Task<AttendanceResponseDto>
        CheckOutAsync(
            int employeeId);

    Task<List<AttendanceResponseDto>>
        GetMyAttendanceAsync(
            int employeeId);
    Task<List<AttendanceResponseDto>>
    GetEmployeeAttendanceAsync(
        int employeeId);
    Task<List<AttendanceResponseDto>>
    GetMonthlyAttendanceAsync(
        int employeeId,
        int year,
        int month);
}