using Journal.Models;
using System.Text.RegularExpressions;

namespace Journal.Services;


//Appends journal entries in SQLITE database:q
public class JournalService
{
    private readonly DatabaseService _database;

    public JournalService(DatabaseService database)
    {
        _database = database;
    }

    public async Task<JournalEntry?> GetTodayEntryAsync()
    {
        return await _database.GetEntryByDateAsync(DateTime.Today);
    }

    public async Task<JournalEntry?> GetEntryByDateAsync(DateTime date)
    {
        return await _database.GetEntryByDateAsync(date);
    }

    public async Task<int> CreateEntryAsync(JournalEntry entry)
    {
        entry.CreatedAt = DateTime.Now;
        entry.UpdatedAt = DateTime.Now;
        entry.WordCount = CountWords(entry.Content);
        return await _database.InsertJournalEntryAsync(entry);
    }

    public async Task<bool> UpdateEntryAsync(JournalEntry entry)
    {
        entry.UpdatedAt = DateTime.Now;
        entry.WordCount = CountWords(entry.Content);
        return await _database.UpdateJournalEntryAsync(entry);
    }

    public async Task<bool> DeleteEntryAsync(int id)
    {
        return await _database.DeleteJournalEntryAsync(id);
    }

    public async Task<List<JournalEntry>> GetAllEntriesAsync()
    {
        return await _database.GetAllEntriesAsync();
    }

    public async Task<List<JournalEntry>> GetEntriesPagedAsync(int page, int pageSize)
    {
        var allEntries = await _database.GetAllEntriesAsync();
        return allEntries.Skip((page - 1) * pageSize).Take(pageSize).ToList();
    }

    public async Task<int> GetTotalEntriesCountAsync()
    {
        var entries = await _database.GetAllEntriesAsync();
        return entries.Count;
    }

    public async Task<List<JournalEntry>> SearchEntriesAsync(string searchTerm)
    {
        var allEntries = await _database.GetAllEntriesAsync();
        searchTerm = searchTerm.ToLower();

        return allEntries.Where(e =>
            e.Title.ToLower().Contains(searchTerm) ||
            e.Content.ToLower().Contains(searchTerm)
        ).ToList();
    }

    public async Task<List<JournalEntry>> FilterEntriesAsync(
        DateTime? startDate = null,
        DateTime? endDate = null,
        List<string>? moods = null,
        List<string>? tags = null)
    {
        var allEntries = await _database.GetAllEntriesAsync();

        if (startDate.HasValue)
            allEntries = allEntries.Where(e => e.Date.Date >= startDate.Value.Date).ToList();

        if (endDate.HasValue)
            allEntries = allEntries.Where(e => e.Date.Date <= endDate.Value.Date).ToList();

        if (moods != null && moods.Count > 0)
        {
            allEntries = allEntries.Where(e =>
                moods.Contains(e.PrimaryMood) ||
                (e.SecondaryMood1 != null && moods.Contains(e.SecondaryMood1)) ||
                (e.SecondaryMood2 != null && moods.Contains(e.SecondaryMood2))
            ).ToList();
        }

        if (tags != null && tags.Count > 0)
        {
            allEntries = allEntries.Where(e =>
                e.GetTags().Any(t => tags.Contains(t))
            ).ToList();
        }

        return allEntries;
    }

    public async Task<List<JournalEntry>> GetEntriesInRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _database.GetEntriesInRangeAsync(startDate, endDate);
    }

    private int CountWords(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return 0;

        // Remove HTML tags
        var plainText = Regex.Replace(text, "<.*?>", string.Empty);

        // Count words
        var words = plainText.Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries);
        return words.Length;
    }

    public async Task<JournalEntry?> GetEntryByIdAsync(int id)
    {
        var allEntries = await _database.GetAllEntriesAsync();
        return allEntries.FirstOrDefault(e => e.Id == id);
    }
}