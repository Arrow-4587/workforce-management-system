using System.ComponentModel.DataAnnotations;

namespace WMS.Application.DTOs.Announcement;

public class CreateAnnouncementDto
{
    [Required]
    [StringLength(200)]
    public string Title { get; set; }
        = string.Empty;

    [Required]
    [StringLength(2000)]
    public string Message { get; set; }
        = string.Empty;
}