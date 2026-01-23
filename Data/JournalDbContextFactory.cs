using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Journal.Data
{
    public class JournalDbContextFactory : IDesignTimeDbContextFactory<JournalDbContext>
    {
        public JournalDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<JournalDbContext>();

            // Use a temporary path for design-time operations
            var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "journal.db");

            optionsBuilder.UseSqlite($"Data Source={dbPath}");

            return new JournalDbContext(optionsBuilder.Options);
        }
    }
}