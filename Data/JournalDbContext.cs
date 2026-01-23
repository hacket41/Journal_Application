using System;
using System.Collections.Generic;
using System.Text;
using Journal.Models;
using Microsoft.EntityFrameworkCore;

namespace Journal.Data
{
    public class JournalDbContext : DbContext
    {
        public JournalDbContext(DbContextOptions<JournalDbContext> options) : base(options)
        {
        }

        public DbSet<JournalEntry> JournalEntries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<JournalEntry>(entity =>
            {
                entity.Property(e => e.Title).HasMaxLength(200);
                entity.Property(e => e.PrimaryMood).HasMaxLength(50);
                entity.Property(e => e.Category).HasMaxLength(50);

                entity.HasIndex(e => e.EntryDate).IsUnique();
            });
        }

    }
}