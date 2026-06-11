using Microsoft.EntityFrameworkCore;
using WMS.Domain.Entities;
using WMS.Infrastructure.Data;
using WMS.Domain.Interfaces;

namespace WMS.Infrastructure.Repositories;

public class AnnouncementRepository
    : IAnnouncementRepository
{
    private readonly WmsDbContext _context;

    public AnnouncementRepository(
        WmsDbContext context)
    {
        _context = context;
    }

    public async Task<List<Announcement>>
        GetAllAsync()
    {
        return await _context.Announcements
            .Include(a => a.Creator)
            .OrderByDescending(a => a.CreatedOn)
            .ToListAsync();
    }

    public async Task<Announcement?>
        GetByIdAsync(
            int announcementId)
    {
        return await _context.Announcements
            .Include(a => a.Creator)
            .FirstOrDefaultAsync(a =>
                a.AnnouncementId == announcementId);
    }

    public async Task AddAsync(
        Announcement announcement)
    {
        await _context.Announcements
            .AddAsync(announcement);

        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(
        Announcement announcement)
    {
        _context.Announcements
            .Update(announcement);

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(
        Announcement announcement)
    {
        _context.Announcements
            .Remove(announcement);

        await _context.SaveChangesAsync();
    }
}