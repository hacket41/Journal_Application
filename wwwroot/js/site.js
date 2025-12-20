// Settings management for journal app
window.loadSettings = function () {
    const defaultSettings = {
        theme: 'light',
        accent: '#5e3a8e',
        fontSize: 'Medium'
    };

    const saved = localStorage.getItem('journalSettings');
    return saved ? JSON.parse(saved) : defaultSettings;
};

window.saveSettings = function (settings) {
    localStorage.setItem('journalSettings', JSON.stringify(settings));
};

// Theme application
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

    // Save setting
    const settings = loadSettings();
    settings.theme = theme;
    saveSettings(settings);
};

// Accent color
window.setAccentColor = function (color) {
    document.documentElement.style.setProperty('--accent-color', color);

    const settings = loadSettings();
    settings.accent = color;
    saveSettings(settings);
};

// Font size
window.setFontSize = function (size) {
    const root = document.documentElement;
    const sizeMap = {
        'Small': '14px',
        'Medium': '16px',
        'Large': '18px',
        'Extra Large': '20px'
    };

    root.style.setProperty('--base-font-size', sizeMap[size] || '16px');

    const settings = loadSettings();
    settings.fontSize = size;
    saveSettings(settings);
};

// Initialize on page load
(function () {
    const settings = loadSettings();
    applyTheme(settings.theme);
    setAccentColor(settings.accent);
    setFontSize(settings.fontSize);
})();