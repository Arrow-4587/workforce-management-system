namespace WMS.Application.Services;

public interface IReportService
{
    Task<byte[]> GenerateExcelReportAsync(string reportType);
}
