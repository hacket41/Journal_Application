namespace Journal.Models;

public class JournalEntry
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string PrimaryMood { get; set; } = string.Empty;
    public string? SecondaryMood1 { get; set; }
    public string? SecondaryMood2 { get; set; }
    public string? Category { get; set; }
    public string Tags { get; set; } = string.Empty; 
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int WordCount { get; set; }

    public List<string> GetTags() =>
        string.IsNullOrWhiteSpace(Tags)
            ? new List<string>()
            : Tags.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();

    public void SetTags(List<string> tags) =>
        Tags = string.Join(",", tags);

    public MoodCategory GetMoodCategory()
    {
        return MoodDefinitions.GetCategory(PrimaryMood);
    }
}

public enum MoodCategory
{
    Positive,
    Neutral,
    Negative
}

public static class MoodDefinitions
{
    public static readonly Dictionary<MoodCategory, List<string>> Moods = new()
    {
        { MoodCategory.Positive, new List<string> { "Happy", "Excited", "Relaxed", "Grateful", "Confident" } },
        { MoodCategory.Neutral, new List<string> { "Calm", "Thoughtful", "Curious", "Nostalgic", "Bored" } },
        { MoodCategory.Negative, new List<string> { "Sad", "Angry", "Stressed", "Lonely", "Anxious" } }
    };

    public static MoodCategory GetCategory(string mood)
    {
        foreach (var kvp in Moods)
        {
            if (kvp.Value.Contains(mood))
                return kvp.Key;
        }
        return MoodCategory.Neutral;
    }

    public static List<string> GetAllMoods()
    {
        return Moods.Values.SelectMany(x => x).ToList();
    }
}

public static class PredefinedTags
{
    public static readonly List<string> Tags = new()
    {
        "Work", "Career", "Studies", "Family", "Friends", "Relationships",
        "Health", "Fitness", "Personal Growth", "Self-care", "Hobbies",
        "Travel", "Nature", "Finance", "Spirituality", "Birthday",
        "Holiday", "Vacation", "Celebration", "Exercise", "Reading",
        "Writing", "Cooking", "Meditation", "Yoga", "Music", "Shopping",
        "Parenting", "Projects", "Planning", "Reflection"
    };
}

public class StreakInfo
{
    public int CurrentStreak { get; set; }
    public int LongestStreak { get; set; }
    public List<DateTime> MissedDays { get; set; } = new();
}

public class MoodDistribution
{
    public int PositiveCount { get; set; }
    public int NeutralCount { get; set; }
    public int NegativeCount { get; set; }
    public int Total => PositiveCount + NeutralCount + NegativeCount;

    public double PositivePercent => Total > 0 ? (PositiveCount * 100.0 / Total) : 0;
    public double NeutralPercent => Total > 0 ? (NeutralCount * 100.0 / Total) : 0;
    public double NegativePercent => Total > 0 ? (NegativeCount * 100.0 / Total) : 0;
}

public class TagUsage
{
    public string Tag { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class WordCountTrend
{
    public DateTime Date { get; set; }
    public double AverageWordCount { get; set; }
}

public class AnalyticsData
{
    public MoodDistribution MoodDistribution { get; set; } = new();
    public string? MostFrequentMood { get; set; }
    public StreakInfo StreakInfo { get; set; } = new();
    public List<TagUsage> MostUsedTags { get; set; } = new();
    public List<WordCountTrend> WordCountTrends { get; set; } = new();
}