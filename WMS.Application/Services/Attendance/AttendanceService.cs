using WMS.Application.DTOs.Attendance;
using WMS.Domain.Interfaces;
using AttendanceEntity =
    WMS.Domain.Entities.Attendance;

namespace WMS.Application.Services.Attendance;

public class AttendanceService
    : IAttendanceService
{
    private readonly IAttendanceRepository
        _attendanceRepository;

    public AttendanceService(
        IAttendanceRepository attendanceRepository)
    {
        _attendanceRepository =
            attendanceRepository;
    }

    public async Task<AttendanceResponseDto>
        CheckInAsync(
            int employeeId,
            CheckInDto dto)
    {
        var todayAttendance =
            await _attendanceRepository
                .GetTodayAttendanceAsync(
                    employeeId);

        if (todayAttendance != null)
        {
            throw new Exception(
                "Already checked in today.");
        }

        if (dto.WorkMode != "WFH" &&
    dto.WorkMode != "WFO" &&
    dto.WorkMode != "Hybrid")
        {
            throw new Exception(
                "WorkMode must be WFH, WFO or Hybrid.");
        }
        var attendance =
            new AttendanceEntity
            {
                EmpId = employeeId,

                CheckIn =
                    DateTime.UtcNow,

                WorkMode =
                    dto.WorkMode,

                AttendanceDate =
                    DateTime.Today
            };

        await _attendanceRepository
            .AddAsync(attendance);

        return Map(attendance);
    }

    public async Task<AttendanceResponseDto>
        CheckOutAsync(
            int employeeId)
    {
        var attendance =
            await _attendanceRepository
                .GetTodayAttendanceAsync(
                    employeeId);

        if (attendance == null)
        {
            throw new Exception(
                "Check-in not found.");
        }

        if (attendance.CheckOut != null)
        {
            throw new Exception(
                "Already checked out.");
        }

        attendance.CheckOut =
            DateTime.UtcNow;

        attendance.TotalHours =
            (attendance.CheckOut.Value
            - attendance.CheckIn)
            .TotalHours;

        await _attendanceRepository
            .UpdateAsync(attendance);

        return Map(attendance);
    }

    public async Task<
        List<AttendanceResponseDto>>
        GetMyAttendanceAsync(
            int employeeId)
    {
        var attendances =
            await _attendanceRepository
                .GetByEmployeeIdAsync(
                    employeeId);

        return attendances
            .Select(Map)
            .ToList();
    }

    public async Task<
    List<AttendanceResponseDto>>
    GetEmployeeAttendanceAsync(
        int employeeId)
    {
        var attendances =
            await _attendanceRepository
                .GetByEmployeeIdAsync(
                    employeeId);

        return attendances
            .Select(Map)
            .ToList();
    }
    public async Task<
    List<AttendanceResponseDto>>
    GetMonthlyAttendanceAsync(
        int employeeId,
        int year,
        int month)
    {
        var attendances =
            await _attendanceRepository
                .GetMonthlyAttendanceAsync(
                    employeeId,
                    year,
                    month);

        return attendances
            .Select(Map)
            .ToList();
    }

    private static AttendanceResponseDto
        Map(
            AttendanceEntity attendance)
    {
        return new AttendanceResponseDto
        {
            AttendanceId =
                attendance.AttendanceId,

            CheckIn =
                attendance.CheckIn,

            CheckOut =
                attendance.CheckOut,

            TotalHours =
                attendance.TotalHours,

            WorkMode =
                attendance.WorkMode,

            AttendanceDate =
                attendance.AttendanceDate
        };
    }
}