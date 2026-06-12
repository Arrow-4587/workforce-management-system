using ClosedXML.Excel;
using WMS.Domain.Interfaces;
using WMS.Application.Services;
using WMS.Domain.Entities;

namespace WMS.Application.Services;

public class ReportService : IReportService
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly ILeaveRepository _leaveRepository;
    private readonly IAttendanceRepository _attendanceRepository;
    private readonly IEmployeeProjectRepository _allocationRepository;

    public ReportService(
        IEmployeeRepository employeeRepository,
        ILeaveRepository leaveRepository,
        IAttendanceRepository attendanceRepository,
        IEmployeeProjectRepository allocationRepository)
    {
        _employeeRepository = employeeRepository;
        _leaveRepository = leaveRepository;
        _attendanceRepository = attendanceRepository;
        _allocationRepository = allocationRepository;
    }

    public async Task<byte[]> GenerateExcelReportAsync(string reportType)
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Report Data");
        
        switch (reportType.ToLower())
        {
            case "employee":
                await GenerateEmployeeReportAsync(ws);
                break;
            case "leave":
                await GenerateLeaveReportAsync(ws);
                break;
            case "attendance":
                await GenerateAttendanceReportAsync(ws);
                break;
            case "allocation":
                await GenerateAllocationReportAsync(ws);
                break;
            default:
                throw new ArgumentException("Invalid report type");
        }

        ws.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    private async Task GenerateEmployeeReportAsync(IXLWorksheet ws)
    {
        ws.Cell(1, 1).Value = "Employee ID";
        ws.Cell(1, 2).Value = "First Name";
        ws.Cell(1, 3).Value = "Last Name";
        ws.Cell(1, 4).Value = "Email";
        ws.Cell(1, 5).Value = "Phone Number";
        ws.Cell(1, 6).Value = "Hire Date";
        ws.Cell(1, 7).Value = "Status";
        ws.Cell(1, 8).Value = "Department";
        ws.Cell(1, 9).Value = "Role";
        StyleHeader(ws, 9);

        var employees = await _employeeRepository.GetAllAsync();
        int row = 2;
        foreach (var emp in employees)
        {
            ws.Cell(row, 1).Value = emp.EmployeeId;
            ws.Cell(row, 2).Value = emp.FirstName;
            ws.Cell(row, 3).Value = emp.LastName;
            ws.Cell(row, 4).Value = emp.Email;
            ws.Cell(row, 5).Value = emp.PhoneNumber;
            ws.Cell(row, 6).Value = emp.DOJ.ToString("yyyy-MM-dd");
            ws.Cell(row, 7).Value = emp.Status;
            ws.Cell(row, 8).Value = emp.Department?.DepartmentName ?? "-";
            ws.Cell(row, 9).Value = emp.Role?.RoleName ?? "-";
            row++;
        }
    }

    private async Task GenerateLeaveReportAsync(IXLWorksheet ws)
    {
        ws.Cell(1, 1).Value = "Leave ID";
        ws.Cell(1, 2).Value = "Employee Name";
        ws.Cell(1, 3).Value = "Leave Type";
        ws.Cell(1, 4).Value = "Start Date";
        ws.Cell(1, 5).Value = "End Date";
        ws.Cell(1, 6).Value = "Status";
        ws.Cell(1, 7).Value = "Applied On";
        ws.Cell(1, 8).Value = "Reason";
        StyleHeader(ws, 8);

        var leaves = await _leaveRepository.GetAllAsync();
        int row = 2;
        foreach (var l in leaves)
        {
            ws.Cell(row, 1).Value = l.LeaveId;
            ws.Cell(row, 2).Value = l.Employee != null ? $"{l.Employee.FirstName} {l.Employee.LastName}" : "-";
            ws.Cell(row, 3).Value = l.LeaveType;
            ws.Cell(row, 4).Value = l.FromDate.ToString("yyyy-MM-dd");
            ws.Cell(row, 5).Value = l.ToDate.ToString("yyyy-MM-dd");
            ws.Cell(row, 6).Value = l.Status;
            ws.Cell(row, 7).Value = l.AppliedOn.ToString("yyyy-MM-dd HH:mm");
            ws.Cell(row, 8).Value = l.Reason;
            row++;
        }
    }

    private async Task GenerateAttendanceReportAsync(IXLWorksheet ws)
    {
        ws.Cell(1, 1).Value = "Attendance ID";
        ws.Cell(1, 2).Value = "Employee Name";
        ws.Cell(1, 3).Value = "Date";
        ws.Cell(1, 4).Value = "Check-In";
        ws.Cell(1, 5).Value = "Check-Out";
        ws.Cell(1, 6).Value = "Work Mode";
        StyleHeader(ws, 6);

        var records = await _attendanceRepository.GetAllAsync();
        int row = 2;
        foreach (var a in records)
        {
            ws.Cell(row, 1).Value = a.AttendanceId;
            ws.Cell(row, 2).Value = a.Employee != null ? $"{a.Employee.FirstName} {a.Employee.LastName}" : "-";
            ws.Cell(row, 3).Value = a.AttendanceDate.ToString("yyyy-MM-dd");
            ws.Cell(row, 4).Value = a.CheckIn.ToString(@"hh\:mm\:ss") ?? "-";
            ws.Cell(row, 5).Value = a.CheckOut?.ToString(@"hh\:mm\:ss") ?? "-";
            ws.Cell(row, 6).Value = a.WorkMode;
            row++;
        }
    }

    private async Task GenerateAllocationReportAsync(IXLWorksheet ws)
    {
        ws.Cell(1, 1).Value = "Allocation ID";
        ws.Cell(1, 2).Value = "Employee Name";
        ws.Cell(1, 3).Value = "Project Name";
        ws.Cell(1, 4).Value = "Allocated On";
        ws.Cell(1, 5).Value = "Released On";
        StyleHeader(ws, 5);

        var allocs = await _allocationRepository.GetAllAsync();
        int row = 2;
        foreach (var ep in allocs)
        {
            ws.Cell(row, 1).Value = ep.AllocationId;
            ws.Cell(row, 2).Value = ep.Employee != null ? $"{ep.Employee.FirstName} {ep.Employee.LastName}" : "-";
            ws.Cell(row, 3).Value = ep.Project?.ProjectName ?? "-";
            ws.Cell(row, 4).Value = ep.AllocatedOn.ToString("yyyy-MM-dd");
            ws.Cell(row, 5).Value = ep.ReleasedOn?.ToString("yyyy-MM-dd") ?? "-";
            row++;
        }
    }

    private void StyleHeader(IXLWorksheet ws, int cols)
    {
        var headerRange = ws.Range(1, 1, 1, cols);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
        headerRange.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
    }
}
