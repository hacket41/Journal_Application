using Journal.Data;
using Journal.Models;
using Journal.Data;
using Journal.Models;
using Microsoft.EntityFrameworkCore;

namespace Journal.Services
{
    public interface IJournalService
    {
        Task<JournalEntry?> GetEntryByDateAsync(DateTime date);
        Task<List<JournalEntry>> GetEntriesAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task SaveEntryAsync(JournalEntry entry);
        Task DeleteEntryAsync(int id);
    }

    public class JournalService : IJournalService
    {
        private readonly AppDbContext _context;

        public JournalService(AppDbContext context)
        {
            _context = context;
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
            // Convert to UTC for storage
            entry.CreatedDate = DateTime.UtcNow;
            entry.EntryDate = DateTime.SpecifyKind(entry.EntryDate, DateTimeKind.Utc);

            if (entry.Id == 0)
            {
                _context.JournalEntries.Add(entry);
            }
            else
            {
                _context.JournalEntries.Update(entry);
                _context.Entry(entry).State = EntityState.Modified;
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