namespace WMS.Application.DTOs.AuditLog;

public class AuditLogResponseDto
{
    public int AuditId { get; set; }

    public string EntityName { get; set; }
        = string.Empty;

    public int RecordId { get; set; }

    public string Action { get; set; }
        = string.Empty;

    public int CreatedBy { get; set; }

    public string Username { get; set; }
        = string.Empty;

    public DateTime CreatedOn { get; set; }
}