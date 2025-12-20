// Apply theme, accent, font size
window.applyTheme = (theme) => {
    document.body.dataset.theme = theme;
    localStorage.setItem('theme', theme);
};

window.setAccentColor = (color) => {
    document.documentElement.style.setProperty('--accent-color', color);
    localStorage.setItem('accent', color);
};

window.setFontSize = (size) => {
    const mapping = { Small: '14px', Medium: '16px', Large: '18px', "Extra Large": '20px' };
    document.documentElement.style.setProperty('--font-size', mapping[size] || '16px');
    localStorage.setItem('fontSize', size);
};

// Load saved settings on page load
window.loadSettings = () => {
    const savedTheme = localStorage.getItem('theme') || 'light';
    document.body.dataset.theme = savedTheme;

    const savedAccent = localStorage.getItem('accent') || '#5e3a8e';
    document.documentElement.style.setProperty('--accent-color', savedAccent);

    const savedFontSize = localStorage.getItem('fontSize') || 'Medium';
    const mapping = { Small: '14px', Medium: '16px', Large: '18px', "Extra Large": '20px' };
    document.documentElement.style.setProperty('--font-size', mapping[savedFontSize] || '16px');

    return {
        theme: savedTheme,
        accent: savedAccent,
        fontSize: savedFontSize
    };
};
