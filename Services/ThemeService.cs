namespace Journal.Services;

public class ThemeService
{
    private readonly DatabaseService _database;
    private const string ThemeKey = "app_theme";

    public event Action? OnThemeChanged;
    public string CurrentTheme { get; private set; } = "light";

    public ThemeService(DatabaseService database)
    {
        _database = database;
        LoadThemeAsync();
    }

    private async void LoadThemeAsync()
    {
        var theme = await _database.GetSettingAsync(ThemeKey);
        CurrentTheme = theme ?? "light";
        OnThemeChanged?.Invoke();
    }

    public async Task SetThemeAsync(string theme)
    {
        CurrentTheme = theme;
        await _database.SetSettingAsync(ThemeKey, theme);
        OnThemeChanged?.Invoke();
    }

    public async Task ToggleThemeAsync()
    {
        var newTheme = CurrentTheme == "light" ? "dark" : "light";
        await SetThemeAsync(newTheme);
    }
}