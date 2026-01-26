using Journal.Models;
using System.Text;

namespace Journal.Services;

public class ExportService
{
    private readonly JournalService _journalService;

    public ExportService(JournalService journalService)
    {
        _journalService = journalService;
    }

    public async Task<List<JournalEntry>> GetEntriesForExportAsync(DateTime startDate, DateTime endDate)
    {
        return await _journalService.GetEntriesInRangeAsync(startDate, endDate);
    }

    public async Task<string> GenerateHtmlForDownloadAsync(DateTime startDate, DateTime endDate)
    {
        var entries = await _journalService.GetEntriesInRangeAsync(startDate, endDate);
        return GenerateHtml(entries, startDate, endDate);
    }

    public async Task<string> ExportToPdfAsync(DateTime startDate, DateTime endDate, string destinationPath)
    {
        var entries = await _journalService.GetEntriesInRangeAsync(startDate, endDate);
        var html = GenerateHtml(entries, startDate, endDate);

        var fileName = $"Journal_{startDate:yyyy-MM-dd}_to_{endDate:yyyy-MM-dd}.html";
        var filePath = Path.Combine(destinationPath, fileName);

        await File.WriteAllTextAsync(filePath, html);

        return filePath;
    }

    private string GenerateHtml(List<JournalEntry> entries, DateTime startDate, DateTime endDate)
    {
        var sb = new StringBuilder();

        sb.AppendLine("<!DOCTYPE html>");
        sb.AppendLine("<html>");
        sb.AppendLine("<head>");
        sb.AppendLine("    <meta charset='utf-8'>");
        sb.AppendLine($"    <title>Journal Entries: {startDate:MMM d, yyyy} - {endDate:MMM d, yyyy}</title>");
        sb.AppendLine("    <style>");
        sb.AppendLine(@"
        body {
            font-family: 'Segoe UI', Arial, sans-serif;
            max-width: 800px;
            margin: 40px auto;
            padding: 20px;
            background-color: #f5f5f5;
        }
        h1 {
            color: #4A90E2;
            text-align: center;
            border-bottom: 3px solid #4A90E2;
            padding-bottom: 10px;
        }
        .entry {
            background: white;
            border: 1px solid #ddd;
            border-radius: 8px;
            padding: 20px;
            margin: 20px 0;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
            page-break-inside: avoid;
        }
        .entry-date {
            font-size: 18px;
            font-weight: bold;
            color: #333;
            margin-bottom: 10px;
        }
        .entry-title {
            font-size: 16px;
            font-weight: bold;
            margin: 10px 0;
        }
        .mood {
            display: inline-block;
            background: #e3f2fd;
            color: #1976d2;
            padding: 5px 12px;
            border-radius: 15px;
            font-size: 14px;
            margin: 5px 5px 5px 0;
        }
        .mood.positive {
            background: #d4edda;
            color: #155724;
        }
        .mood.neutral {
            background: #fff3cd;
            color: #856404;
        }
        .mood.negative {
            background: #f8d7da;
            color: #721c24;
        }
        .tag {
            display: inline-block;
            background: #4A90E2;
            color: white;
            padding: 4px 10px;
            border-radius: 12px;
            font-size: 12px;
            margin: 3px;
        }
        .content {
            margin: 15px 0;
            line-height: 1.6;
            white-space: pre-wrap;
        }
        .metadata {
            font-size: 12px;
            color: #666;
            margin-top: 10px;
            padding-top: 10px;
            border-top: 1px solid #eee;
        }
        .no-entries {
            text-align: center;
            color: #666;
            font-style: italic;
            padding: 40px;
        }
        @media print {
            body {
                background: white;
                margin: 0;
            }
            .entry {
                box-shadow: none;
            }
        }
        ");
        sb.AppendLine("    </style>");
        sb.AppendLine("</head>");
        sb.AppendLine("<body>");

        sb.AppendLine($"    <h1>Journal Entries: {startDate:MMM d, yyyy} - {endDate:MMM d, yyyy}</h1>");

        if (!entries.Any())
        {
            sb.AppendLine("    <div class='no-entries'>No entries found for this date range.</div>");
        }
        else
        {
            foreach (var entry in entries.OrderBy(e => e.Date))
            {
                sb.AppendLine("    <div class='entry'>");
                sb.AppendLine($"        <div class='entry-date'>{entry.Date:dddd, MMMM d, yyyy}</div>");

                if (!string.IsNullOrEmpty(entry.Title))
                {
                    sb.AppendLine($"        <div class='entry-title'>{System.Net.WebUtility.HtmlEncode(entry.Title)}</div>");
                }

                // Moods
                var moodCategory = entry.GetMoodCategory().ToString().ToLower();
                sb.AppendLine("        <div>");
                sb.AppendLine($"            <span class='mood {moodCategory}'>{System.Net.WebUtility.HtmlEncode(entry.PrimaryMood)}</span>");

                if (!string.IsNullOrEmpty(entry.SecondaryMood1))
                {
                    sb.AppendLine($"            <span class='mood'>{System.Net.WebUtility.HtmlEncode(entry.SecondaryMood1)}</span>");
                }

                if (!string.IsNullOrEmpty(entry.SecondaryMood2))
                {
                    sb.AppendLine($"            <span class='mood'>{System.Net.WebUtility.HtmlEncode(entry.SecondaryMood2)}</span>");
                }
                sb.AppendLine("        </div>");

                // Tags
                var tags = entry.GetTags();
                if (tags.Any())
                {
                    sb.AppendLine("        <div style='margin-top: 10px;'>");
                    foreach (var tag in tags)
                    {
                        sb.AppendLine($"            <span class='tag'>{System.Net.WebUtility.HtmlEncode(tag)}</span>");
                    }
                    sb.AppendLine("        </div>");
                }

                // Content
                var encodedContent = System.Net.WebUtility.HtmlEncode(entry.Content);
                sb.AppendLine($"        <div class='content'>{encodedContent}</div>");

                // Metadata
                sb.AppendLine($"        <div class='metadata'>Words: {entry.WordCount} | Created: {entry.CreatedAt:g}</div>");

                sb.AppendLine("    </div>");
            }
        }

        sb.AppendLine("</body>");
        sb.AppendLine("</html>");

        return sb.ToString();
    }
}