using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Journal.Models
{
    public class JournalEntry
    {
        [Key]
        public int Id { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime EntryDate { get; set; }

        public string? Title { get; set; }

        [Required]
        public string Content { get; set; } = string.Empty;

        [Required]
        public string PrimaryMood { get; set; } = "Neutral";

        public string? SecondaryMoods { get; set; } 
        public string? Category { get; set; }
        public string? Tags { get; set; } 
    }
}