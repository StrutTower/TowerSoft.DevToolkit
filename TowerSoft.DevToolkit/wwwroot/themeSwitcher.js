var currentTheme;
updateTheme('auto');

// Updates to the supplied theme
function updateTheme(theme) {
    currentTheme = theme;
    if (theme === null || theme === 'auto') {
        theme = getPreferedTheme();
    }
    document.documentElement.setAttribute('data-bs-theme', theme);
}

function getCurrentThemeSetting() {
    if (currentTheme === null || currentTheme === 'auto') {
        return 'auto';
    }
    return currentTheme;
}

// Returns the prefers-color-scheme setting 
function getPreferedTheme() {
    return window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light';
}

// Add an event listener if prefers-color-scheme is updated.
window.matchMedia('(prefers-color-scheme: dark)')
    .addEventListener('change', ({ matches }) => {
        //const theme = localStorage.getItem('preferred-theme');
        if (currentTheme === null || currentTheme === 'auto') {
            updateTheme(getPreferedTheme());
        }
    });