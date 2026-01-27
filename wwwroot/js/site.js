// site.js - Theme, Accent, and Font Settings for Journal App

// Load saved settings from localStorage or return defaults
window.loadSettings = function () {
    const defaultSettings = {
        theme: 'light',       // default theme
        accent: '#5e3a8e',    // default accent color
        fontSize: 'Medium'    // default font size
    };

    const saved = localStorage.getItem('journalSettings');
    return saved ? JSON.parse(saved) : defaultSettings;
};

// Save settings to localStorage
window.saveSettings = function (settings) {
    localStorage.setItem('journalSettings', JSON.stringify(settings));
};

// Apply a theme (light, dark, auto)
window.applyTheme = function (theme) {
    const root = document.documentElement;

    // Remove existing theme classes
    root.classList.remove('light', 'dark', 'auto');

    if (theme === 'auto') {
        // Detect system preference
        const prefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
        root.classList.add(prefersDark ? 'dark' : 'light');
    } else {
        root.classList.add(theme);
    }

    // Save the theme in settings
    const settings = window.loadSettings();
    settings.theme = theme;
    window.saveSettings(settings);
};

// Set accent color
window.setAccentColor = function (color) {
    document.documentElement.style.setProperty('--accent-color', color);
    const settings = window.loadSettings();
    settings.accent = color;
    window.saveSettings(settings);
};

// Set font size
window.setFontSize = function (size) {
    const root = document.documentElement;
    const sizeMap = {
        'Small': '14px',
        'Medium': '16px',
        'Large': '18px',
        'Extra Large': '20px'
    };

    root.style.setProperty('--base-font-size', sizeMap[size] || '16px');

    const settings = window.loadSettings();
    settings.fontSize = size;
    window.saveSettings(settings);
};

// Initialize on page load
(function () {
    const settings = window.loadSettings();

    // Apply saved theme
    window.applyTheme(settings.theme);

    // Apply saved accent color
    window.setAccentColor(settings.accent);

    // Apply saved font size
    window.setFontSize(settings.fontSize);
})();
