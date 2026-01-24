using Journal.Models;
using Microsoft.JSInterop;

namespace Journal.Services;

public class ThemeService
{
    private readonly DatabaseService _databaseService;
    private readonly IJSRuntime _jsRuntime;
    private const string THEME_KEY = "app_theme";
    private const string CUSTOM_COLORS_KEY = "custom_colors";

    public ThemeService(DatabaseService databaseService, IJSRuntime jsRuntime)
    {
        _databaseService = databaseService;
        _jsRuntime = jsRuntime;
    }

    public async Task<string> GetCurrentThemeAsync()
    {
        var theme = await _databaseService.GetSettingAsync(THEME_KEY);
        return theme ?? "light";
    }

    public async Task SetThemeAsync(string theme)
    {
        await _databaseService.SetSettingAsync(THEME_KEY, theme);
        await ApplyThemeToUI(theme);
    }

    public async Task ApplyCustomThemeAsync(string primaryColor, string backgroundColor, string textColor, string cardBackground)
    {
        var customColors = new CustomThemeColors
        {
            PrimaryColor = primaryColor,
            BackgroundColor = backgroundColor,
            TextColor = textColor,
            CardBackground = cardBackground
        };

        var json = System.Text.Json.JsonSerializer.Serialize(customColors);
        await _databaseService.SetSettingAsync(CUSTOM_COLORS_KEY, json);
        await _databaseService.SetSettingAsync(THEME_KEY, "custom");

        await ApplyCustomColorsToUI(customColors);
    }

    public async Task<CustomThemeColors?> GetCustomColorsAsync()
    {
        var json = await _databaseService.GetSettingAsync(CUSTOM_COLORS_KEY);
        if (string.IsNullOrEmpty(json))
            return null;

        return System.Text.Json.JsonSerializer.Deserialize<CustomThemeColors>(json);
    }

    private async Task ApplyThemeToUI(string theme)
    {
        try
        {
            if (theme == "custom")
            {
                var customColors = await GetCustomColorsAsync();
                if (customColors != null)
                {
                    await ApplyCustomColorsToUI(customColors);
                    return;
                }
            }

            await _jsRuntime.InvokeVoidAsync("applyTheme", theme);
        }
        catch (Exception)
        {
            // Handle JS interop errors silently
        }
    }

    private async Task ApplyCustomColorsToUI(CustomThemeColors colors)
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("applyCustomTheme",
                colors.PrimaryColor,
                colors.BackgroundColor,
                colors.TextColor,
                colors.CardBackground
            );
        }
        catch (Exception)
        {
            // Handle JS interop errors silently
        }
    }

    public async Task InitializeThemeAsync()
    {
        var currentTheme = await GetCurrentThemeAsync();
        await ApplyThemeToUI(currentTheme);
    }
}

public class CustomThemeColors
{
    public string PrimaryColor { get; set; } = "#6366f1";
    public string BackgroundColor { get; set; } = "#ffffff";
    public string TextColor { get; set; } = "#1f2937";
    public string CardBackground { get; set; } = "#f9fafb";
}