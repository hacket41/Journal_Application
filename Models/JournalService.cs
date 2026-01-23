using Journal.Data;
using Journal.Models;
using Microsoft.EntityFrameworkCore;

namespace Journal.Services
{
    public interface IJournalService
    {
        Task<JournalEntry?> GetEntryByIdAsync(int id);
        Task<JournalEntry?> GetEntryByDateAsync(DateTime date);
        Task<List<JournalEntry>> GetEntriesAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task SaveEntryAsync(JournalEntry entry);
        Task DeleteEntryAsync(int id);
    }

    public class JournalService : IJournalService
    {
        private readonly JournalDbContext _context;

        public JournalService(JournalDbContext context)
        {
            _context = context;
        }

        public async Task<JournalEntry?> GetEntryByIdAsync(int id)
        {
            return await _context.JournalEntries.FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<JournalEntry?> GetEntryByDateAsync(DateTime date)
        {
            var localDate = date.Date;
            return await _context.JournalEntries
                .FirstOrDefaultAsync(e => e.EntryDate.Date == localDate);
        }

        public async Task<List<JournalEntry>> GetEntriesAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.JournalEntries.AsQueryable();

            if (startDate.HasValue)
                query = query.Where(e => e.EntryDate >= startDate.Value.Date);

            if (endDate.HasValue)
                query = query.Where(e => e.EntryDate < endDate.Value.Date.AddDays(1));

            return await query.OrderByDescending(e => e.EntryDate).ToListAsync();
        }

        public async Task SaveEntryAsync(JournalEntry entry)
        {
            var existing = await GetEntryByDateAsync(entry.EntryDate);

            if (existing == null)
            {
                entry.CreatedAt = DateTime.UtcNow;
                entry.UpdatedAt = DateTime.UtcNow;
                entry.EntryDate = entry.EntryDate.Date;

                _context.JournalEntries.Add(entry);
            }
            else
            {
                existing.Title = entry.Title;
                existing.Content = entry.Content;
                existing.PrimaryMood = entry.PrimaryMood;
                existing.SecondaryMoods = entry.SecondaryMoods;
                existing.Tags = entry.Tags;
                existing.Category = entry.Category;
                existing.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteEntryAsync(int id)
        {
            var entry = await _context.JournalEntries.FindAsync(id);
            if (entry != null)
            {
                _context.JournalEntries.Remove(entry);
                await _context.SaveChangesAsync();
            }
        }
    }
}
