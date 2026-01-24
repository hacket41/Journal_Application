window.setThemeColors = (themeColors) => {
    const root = document.documentElement;

    if (themeColors.PrimaryColor)
        root.style.setProperty('--primary-color', themeColors.PrimaryColor);
    if (themeColors.BackgroundColor)
        root.style.setProperty('--background-color', themeColors.BackgroundColor);
    if (themeColors.TextColor)
        root.style.setProperty('--text-color', themeColors.TextColor);
    if (themeColors.CardBackground)
        root.style.setProperty('--card-background', themeColors.CardBackground);
};
