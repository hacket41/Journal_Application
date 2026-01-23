using Microsoft.Data.Sqlite;
using Journal.Models;

namespace Journal.Services;

public class DatabaseService
{
    private readonly string _dbPath;
    private readonly string _connectionString;

    public DatabaseService()
    {
        var appDataPath = FileSystem.AppDataDirectory;
        _dbPath = Path.Combine(appDataPath, "journal.db");
        _connectionString = $"Data Source={_dbPath}";

        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var createTableCmd = connection.CreateCommand();
        createTableCmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS JournalEntries (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Date TEXT NOT NULL UNIQUE,
                Title TEXT NOT NULL,
                Content TEXT NOT NULL,
                PrimaryMood TEXT NOT NULL,
                SecondaryMood1 TEXT,
                SecondaryMood2 TEXT,
                Category TEXT,
                Tags TEXT,
                CreatedAt TEXT NOT NULL,
                UpdatedAt TEXT NOT NULL,
                WordCount INTEGER NOT NULL
            );

            CREATE TABLE IF NOT EXISTS Settings (
                Key TEXT PRIMARY KEY,
                Value TEXT NOT NULL
            );";
        createTableCmd.ExecuteNonQuery();
    }

    public SqliteConnection GetConnection()
    {
        return new SqliteConnection(_connectionString);
    }

    public async Task<int> InsertJournalEntryAsync(JournalEntry entry)
    {
        using var connection = GetConnection();
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO JournalEntries (Date, Title, Content, PrimaryMood, SecondaryMood1, SecondaryMood2, Category, Tags, CreatedAt, UpdatedAt, WordCount)
            VALUES (@Date, @Title, @Content, @PrimaryMood, @SecondaryMood1, @SecondaryMood2, @Category, @Tags, @CreatedAt, @UpdatedAt, @WordCount);
            SELECT last_insert_rowid();";

        command.Parameters.AddWithValue("@Date", entry.Date.Date.ToString("yyyy-MM-dd"));
        command.Parameters.AddWithValue("@Title", entry.Title);
        command.Parameters.AddWithValue("@Content", entry.Content);
        command.Parameters.AddWithValue("@PrimaryMood", entry.PrimaryMood);
        command.Parameters.AddWithValue("@SecondaryMood1", entry.SecondaryMood1 ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@SecondaryMood2", entry.SecondaryMood2 ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@Category", entry.Category ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@Tags", entry.Tags);
        command.Parameters.AddWithValue("@CreatedAt", entry.CreatedAt.ToString("o"));
        command.Parameters.AddWithValue("@UpdatedAt", entry.UpdatedAt.ToString("o"));
        command.Parameters.AddWithValue("@WordCount", entry.WordCount);

        var result = await command.ExecuteScalarAsync();
        return Convert.ToInt32(result);
    }

    public async Task<bool> UpdateJournalEntryAsync(JournalEntry entry)
    {
        using var connection = GetConnection();
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = @"
            UPDATE JournalEntries 
            SET Title = @Title, Content = @Content, PrimaryMood = @PrimaryMood, 
                SecondaryMood1 = @SecondaryMood1, SecondaryMood2 = @SecondaryMood2,
                Category = @Category, Tags = @Tags, UpdatedAt = @UpdatedAt, WordCount = @WordCount
            WHERE Id = @Id";

        command.Parameters.AddWithValue("@Id", entry.Id);
        command.Parameters.AddWithValue("@Title", entry.Title);
        command.Parameters.AddWithValue("@Content", entry.Content);
        command.Parameters.AddWithValue("@PrimaryMood", entry.PrimaryMood);
        command.Parameters.AddWithValue("@SecondaryMood1", entry.SecondaryMood1 ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@SecondaryMood2", entry.SecondaryMood2 ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@Category", entry.Category ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@Tags", entry.Tags);
        command.Parameters.AddWithValue("@UpdatedAt", entry.UpdatedAt.ToString("o"));
        command.Parameters.AddWithValue("@WordCount", entry.WordCount);

        var rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }

    public async Task<bool> DeleteJournalEntryAsync(int id)
    {
        using var connection = GetConnection();
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM JournalEntries WHERE Id = @Id";
        command.Parameters.AddWithValue("@Id", id);

        var rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }

    public async Task<JournalEntry?> GetEntryByDateAsync(DateTime date)
    {
        using var connection = GetConnection();
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT * FROM JournalEntries WHERE Date = @Date";
        command.Parameters.AddWithValue("@Date", date.Date.ToString("yyyy-MM-dd"));

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return MapReaderToEntry(reader);
        }

        return null;
    }

    public async Task<List<JournalEntry>> GetAllEntriesAsync()
    {
        var entries = new List<JournalEntry>();
        using var connection = GetConnection();
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT * FROM JournalEntries ORDER BY Date DESC";

        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            entries.Add(MapReaderToEntry(reader));
        }

        return entries;
    }

    public async Task<List<JournalEntry>> GetEntriesInRangeAsync(DateTime startDate, DateTime endDate)
    {
        var entries = new List<JournalEntry>();
        using var connection = GetConnection();
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT * FROM JournalEntries 
            WHERE Date >= @StartDate AND Date <= @EndDate 
            ORDER BY Date DESC";
        command.Parameters.AddWithValue("@StartDate", startDate.Date.ToString("yyyy-MM-dd"));
        command.Parameters.AddWithValue("@EndDate", endDate.Date.ToString("yyyy-MM-dd"));

        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            entries.Add(MapReaderToEntry(reader));
        }

        return entries;
    }

    private JournalEntry MapReaderToEntry(SqliteDataReader reader)
    {
        return new JournalEntry
        {
            Id = reader.GetInt32(0),
            Date = DateTime.Parse(reader.GetString(1)),
            Title = reader.GetString(2),
            Content = reader.GetString(3),
            PrimaryMood = reader.GetString(4),
            SecondaryMood1 = reader.IsDBNull(5) ? null : reader.GetString(5),
            SecondaryMood2 = reader.IsDBNull(6) ? null : reader.GetString(6),
            Category = reader.IsDBNull(7) ? null : reader.GetString(7),
            Tags = reader.GetString(8),
            CreatedAt = DateTime.Parse(reader.GetString(9)),
            UpdatedAt = DateTime.Parse(reader.GetString(10)),
            WordCount = reader.GetInt32(11)
        };
    }

    public async Task<string?> GetSettingAsync(string key)
    {
        using var connection = GetConnection();
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT Value FROM Settings WHERE Key = @Key";
        command.Parameters.AddWithValue("@Key", key);

        var result = await command.ExecuteScalarAsync();
        return result?.ToString();
    }

    public async Task SetSettingAsync(string key, string value)
    {
        using var connection = GetConnection();
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = @"
            INSERT OR REPLACE INTO Settings (Key, Value) 
            VALUES (@Key, @Value)";
        command.Parameters.AddWithValue("@Key", key);
        command.Parameters.AddWithValue("@Value", value);

        await command.ExecuteNonQueryAsync();
    }


}