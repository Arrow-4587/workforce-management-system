using WMS.Domain.Entities;

namespace WMS.Domain.Interfaces;

public interface IAnnouncementRepository
{
    Task<List<Announcement>>
        GetAllAsync();

    Task<Announcement?>
        GetByIdAsync(
            int announcementId);

    Task AddAsync(
        Announcement announcement);

    Task UpdateAsync(
        Announcement announcement);

    Task DeleteAsync(
        Announcement announcement);
}