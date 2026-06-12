using Microsoft.EntityFrameworkCore;
using WMS.Domain.Entities;
using WMS.Domain.Interfaces;
using WMS.Infrastructure.Data;
using WMS.Infrastructure.Repositories;

namespace WMS.Infrastructure.Repositories;

public class AttendanceRepository
    : IAttendanceRepository
{
    private readonly WmsDbContext _context;

    public AttendanceRepository(
        WmsDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(
        Attendance attendance)
    {
        await _context.Attendances
            .AddAsync(attendance);

        await _context
            .SaveChangesAsync();
    }

    public async Task UpdateAsync(
        Attendance attendance)
    {
        _context.Attendances
            .Update(attendance);

        await _context
            .SaveChangesAsync();
    }
    public async Task<List<Attendance>>
    GetMonthlyAttendanceAsync(
        int employeeId,
        int year,
        int month)
    {
        return await _context.Attendances
            .Where(a =>
                a.EmpId == employeeId &&
                a.AttendanceDate.Year == year &&
                a.AttendanceDate.Month == month)
            .OrderBy(a =>
                a.AttendanceDate)
            .ToListAsync();
    }
    public async Task<Attendance?>
        GetTodayAttendanceAsync(
            int employeeId)
    {
        var today =
            DateTime.Today;

        return await _context.Attendances
            .FirstOrDefaultAsync(a =>
                a.EmpId == employeeId
                && a.AttendanceDate
                    == today);
    }

    public async Task<List<Attendance>>
        GetByEmployeeIdAsync(
            int employeeId)
    {
        return await _context.Attendances
            .Where(a =>
                a.EmpId == employeeId)
            .OrderByDescending(a =>
                a.AttendanceDate)
            .ToListAsync();
    }
    public async Task<int>
GetTodayAttendanceCountAsync()
    {
        return await _context.Attendances
            .CountAsync(a =>
                a.AttendanceDate.Date ==
                DateTime.Today);
    }
    public async Task<int>
    GetMonthlyAttendanceCountAsync(
        int employeeId,
        int year,
        int month)
    {
        return await _context.Attendances
            .CountAsync(a =>
                a.EmpId == employeeId &&
                a.AttendanceDate.Year == year &&
                a.AttendanceDate.Month == month);
    }

    public async Task<List<Attendance>> GetAllAsync()
    {
        return await _context.Attendances
            .Include(a => a.Employee)
            .OrderByDescending(a => a.AttendanceDate)
            .ToListAsync();
    }
}
