window.applyTheme = (theme) => {
    const body = document.body;

    body.classList.remove("theme-light", "theme-dark", "theme-custom");

    if (theme === "auto") {
        const prefersDark = window.matchMedia("(prefers-color-scheme: dark)").matches;
        body.classList.add(prefersDark ? "theme-dark" : "theme-light");
    } else {
        body.classList.add(`theme-${theme}`);
    }
};

window.applyCustomTheme = (primary, background, text, card) => {
    const root = document.documentElement;

    root.style.setProperty("--primary-color", primary);
    root.style.setProperty("--background-color", background);
    root.style.setProperty("--text-color", text);
    root.style.setProperty("--card-background", card);

    document.body.classList.remove("theme-light", "theme-dark");
    document.body.classList.add("theme-custom");
};
