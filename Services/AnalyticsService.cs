using Journal.Models;

namespace Journal.Services;

public class AnalyticsService
{
    private readonly JournalService _journalService;

    public AnalyticsService(JournalService journalService)
    {
        _journalService = journalService;
    }

    public async Task<AnalyticsData> GetAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        List<JournalEntry> entries;

        if (startDate.HasValue && endDate.HasValue)
        {
            entries = await _journalService.GetEntriesInRangeAsync(startDate.Value, endDate.Value);
        }
        else
        {
            entries = await _journalService.GetAllEntriesAsync();
        }

        var analytics = new AnalyticsData
        {
            MoodDistribution = CalculateMoodDistribution(entries),
            MostFrequentMood = GetMostFrequentMood(entries),
            StreakInfo = await CalculateStreaksAsync(),
            MostUsedTags = CalculateMostUsedTags(entries),
            WordCountTrends = CalculateWordCountTrends(entries)
        };

        return analytics;
    }

    private MoodDistribution CalculateMoodDistribution(List<JournalEntry> entries)
    {
        var distribution = new MoodDistribution();

        foreach (var entry in entries)
        {
            var category = entry.GetMoodCategory();
            switch (category)
            {
                case MoodCategory.Positive:
                    distribution.PositiveCount++;
                    break;
                case MoodCategory.Neutral:
                    distribution.NeutralCount++;
                    break;
                case MoodCategory.Negative:
                    distribution.NegativeCount++;
                    break;
            }
        }

        return distribution;
    }

    private string? GetMostFrequentMood(List<JournalEntry> entries)
    {
        if (!entries.Any())
            return null;

        var moodCounts = new Dictionary<string, int>();

        foreach (var entry in entries)
        {
            if (!moodCounts.ContainsKey(entry.PrimaryMood))
                moodCounts[entry.PrimaryMood] = 0;
            moodCounts[entry.PrimaryMood]++;

            if (!string.IsNullOrEmpty(entry.SecondaryMood1))
            {
                if (!moodCounts.ContainsKey(entry.SecondaryMood1))
                    moodCounts[entry.SecondaryMood1] = 0;
                moodCounts[entry.SecondaryMood1]++;
            }

            if (!string.IsNullOrEmpty(entry.SecondaryMood2))
            {
                if (!moodCounts.ContainsKey(entry.SecondaryMood2))
                    moodCounts[entry.SecondaryMood2] = 0;
                moodCounts[entry.SecondaryMood2]++;
            }
        }

        return moodCounts.OrderByDescending(x => x.Value).First().Key;
    }

    public async Task<StreakInfo> CalculateStreaksAsync()
    {
        var allEntries = await _journalService.GetAllEntriesAsync();
        var entryDates = allEntries.Select(e => e.Date.Date).OrderByDescending(d => d).ToList();

        var streakInfo = new StreakInfo();

        if (!entryDates.Any())
            return streakInfo;

        // Calculates current streak for analytics 
        var today = DateTime.Today;
        var currentStreak = 0;
        var checkDate = today;

        // Check if there's an entry today or yesterday to start counting
        if (entryDates.Contains(today) || entryDates.Contains(today.AddDays(-1)))
        {
            if (!entryDates.Contains(today))
                checkDate = today.AddDays(-1);

            while (entryDates.Contains(checkDate))
            {
                currentStreak++;
                checkDate = checkDate.AddDays(-1);
            }
        }

        streakInfo.CurrentStreak = currentStreak;

        // Calculates longest streak
        var longestStreak = 0;
        var tempStreak = 1;

        for (int i = 0; i < entryDates.Count - 1; i++)
        {
            if ((entryDates[i] - entryDates[i + 1]).Days == 1)
            {
                tempStreak++;
            }
            else
            {
                longestStreak = Math.Max(longestStreak, tempStreak);
                tempStreak = 1;
            }
        }
        longestStreak = Math.Max(longestStreak, tempStreak);
        streakInfo.LongestStreak = longestStreak;

        // Calculate missed days in the last 30 days
        var thirtyDaysAgo = today.AddDays(-30);
        var missedDays = new List<DateTime>();

        for (var date = thirtyDaysAgo; date <= today; date = date.AddDays(1))
        {
            if (!entryDates.Contains(date))
            {
                missedDays.Add(date);
            }
        }

        streakInfo.MissedDays = missedDays;

        return streakInfo;
    }

    private List<TagUsage> CalculateMostUsedTags(List<JournalEntry> entries)
    {
        var tagCounts = new Dictionary<string, int>();

        foreach (var entry in entries)
        {
            var tags = entry.GetTags();
            foreach (var tag in tags)
            {
                if (!tagCounts.ContainsKey(tag))
                    tagCounts[tag] = 0;
                tagCounts[tag]++;
            }
        }

        return tagCounts
            .Select(kvp => new TagUsage { Tag = kvp.Key, Count = kvp.Value })
            .OrderByDescending(t => t.Count)
            .Take(10)
            .ToList();
    }

    private List<WordCountTrend> CalculateWordCountTrends(List<JournalEntry> entries)
    {
        var trends = new List<WordCountTrend>();

        if (!entries.Any())
            return trends;

        // Group by month
        var monthlyGroups = entries
            .GroupBy(e => new { e.Date.Year, e.Date.Month })
            .OrderBy(g => g.Key.Year)
            .ThenBy(g => g.Key.Month);

        foreach (var group in monthlyGroups)
        {
            var avgWordCount = group.Average(e => e.WordCount);
            var firstDayOfMonth = new DateTime(group.Key.Year, group.Key.Month, 1);

            trends.Add(new WordCountTrend
            {
                Date = firstDayOfMonth,
                AverageWordCount = avgWordCount
            });
        }

        return trends;
    }
}