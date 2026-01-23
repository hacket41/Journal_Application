using System;
using System.ComponentModel.DataAnnotations;

namespace Journal.Models
{
    public class JournalEntry
    {
        [Key]
        public int Id { get; set; }

     
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

       
        public DateTime EntryDate { get; set; }

        [MaxLength(200)]
        public string? Title { get; set; }

        [Required]
        public string Content { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string PrimaryMood { get; set; } = "Neutral";

        public string? SecondaryMoods { get; set; }

        [MaxLength(50)]
        public string? Category { get; set; }

      
        public string? Tags { get; set; }
    }
}
