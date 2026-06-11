using System.ComponentModel.DataAnnotations;
using WMS.Domain.Entities;

public class Announcement
{
    public int AnnouncementId { get; set; }

    [Required]
    [StringLength(200)]
    public string Title { get; set; }
        = string.Empty;

    [Required]
    [StringLength(2000)]
    public string Message { get; set; }
        = string.Empty;

    public int CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public bool IsActive { get; set; }
        = true;

    public UserLogin? Creator { get; set; }
}