// Theme Management Functions

window.applyTheme = function (theme) {
    const root = document.documentElement;

    if (theme === 'dark') {
        root.style.setProperty('--primary-color', '#818cf8');
        root.style.setProperty('--primary-hover', '#6366f1');
        root.style.setProperty('--background-color', '#1f2937');
        root.style.setProperty('--surface-color', '#374151');
        root.style.setProperty('--card-background', '#2d3748');
        root.style.setProperty('--text-color', '#f9fafb');
        root.style.setProperty('--text-secondary', '#d1d5db');
        root.style.setProperty('--border-color', '#4b5563');
        root.style.setProperty('--input-background', '#374151');
        root.style.setProperty('--input-border', '#4b5563');
        root.style.setProperty('--shadow', 'rgba(0, 0, 0, 0.3)');
        root.style.setProperty('--success-color', '#10b981');
        root.style.setProperty('--error-color', '#ef4444');
        root.style.setProperty('--warning-color', '#f59e0b');
    } else if (theme === 'auto') {
        // Check system preference
        const isDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
        applyTheme(isDark ? 'dark' : 'light');
        return;
    } else {
        // Light theme (default)
        root.style.setProperty('--primary-color', '#6366f1');
        root.style.setProperty('--primary-hover', '#4f46e5');
        root.style.setProperty('--background-color', '#f9fafb');
        root.style.setProperty('--surface-color', '#ffffff');
        root.style.setProperty('--card-background', '#ffffff');
        root.style.setProperty('--text-color', '#1f2937');
        root.style.setProperty('--text-secondary', '#6b7280');
        root.style.setProperty('--border-color', '#e5e7eb');
        root.style.setProperty('--input-background', '#ffffff');
        root.style.setProperty('--input-border', '#d1d5db');
        root.style.setProperty('--shadow', 'rgba(0, 0, 0, 0.1)');
        root.style.setProperty('--success-color', '#10b981');
        root.style.setProperty('--error-color', '#ef4444');
        root.style.setProperty('--warning-color', '#f59e0b');
    }

    // Store theme preference
    localStorage.setItem('theme', theme);

    // Update body class for additional styling
    document.body.className = theme === 'dark' ? 'theme-dark' : 'theme-light';
};

window.applyCustomTheme = function (primaryColor, backgroundColor, textColor, cardBackground) {
    const root = document.documentElement;

    root.style.setProperty('--primary-color', primaryColor);
    root.style.setProperty('--primary-hover', adjustColor(primaryColor, -20));
    root.style.setProperty('--background-color', backgroundColor);
    root.style.setProperty('--surface-color', adjustColor(backgroundColor, 5));
    root.style.setProperty('--card-background', cardBackground);
    root.style.setProperty('--text-color', textColor);
    root.style.setProperty('--text-secondary', adjustColor(textColor, 30));
    root.style.setProperty('--border-color', adjustColor(backgroundColor, -10));
    root.style.setProperty('--input-background', cardBackground);
    root.style.setProperty('--input-border', adjustColor(cardBackground, -15));

    // Determine if background is dark or light for shadow
    const isBackgroundDark = isColorDark(backgroundColor);
    root.style.setProperty('--shadow', isBackgroundDark ? 'rgba(0, 0, 0, 0.3)' : 'rgba(0, 0, 0, 0.1)');

    // Keep standard success/error colors
    root.style.setProperty('--success-color', '#10b981');
    root.style.setProperty('--error-color', '#ef4444');
    root.style.setProperty('--warning-color', '#f59e0b');

    // Update body class
    document.body.className = isBackgroundDark ? 'theme-dark theme-custom' : 'theme-light theme-custom';
};

// Helper function to adjust color brightness
function adjustColor(color, percent) {
    const num = parseInt(color.replace('#', ''), 16);
    const amt = Math.round(2.55 * percent);
    const R = (num >> 16) + amt;
    const G = (num >> 8 & 0x00FF) + amt;
    const B = (num & 0x0000FF) + amt;

    return '#' + (
        0x1000000 +
        (R < 255 ? (R < 1 ? 0 : R) : 255) * 0x10000 +
        (G < 255 ? (G < 1 ? 0 : G) : 255) * 0x100 +
        (B < 255 ? (B < 1 ? 0 : B) : 255)
    ).toString(16).slice(1);
}

// Helper function to determine if color is dark
function isColorDark(color) {
    const hex = color.replace('#', '');
    const r = parseInt(hex.substr(0, 2), 16);
    const g = parseInt(hex.substr(2, 2), 16);
    const b = parseInt(hex.substr(4, 2), 16);
    const brightness = (r * 299 + g * 587 + b * 114) / 1000;
    return brightness < 128;
}

// Listen for system theme changes when in auto mode
window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', (e) => {
    const currentTheme = localStorage.getItem('theme');
    if (currentTheme === 'auto') {
        applyTheme('auto');
    }
});

// Initialize theme on page load
document.addEventListener('DOMContentLoaded', function () {
    const savedTheme = localStorage.getItem('theme') || 'light';
    applyTheme(savedTheme);
});