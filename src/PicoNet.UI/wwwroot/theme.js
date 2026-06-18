window.initTheme = () => {
    const savedTheme = localStorage.getItem("nim-theme") || "light";
    const savedPalette = localStorage.getItem("nim-palette") || "default";

    document.documentElement.classList.toggle("dark", savedTheme === "dark");
    document.documentElement.dataset.palette = savedPalette;
};

window.toggleTheme = () => {
    const root = document.documentElement;
    const isDark = root.classList.toggle("dark");
    localStorage.setItem("nim-theme", isDark ? "dark" : "light");
};

window.setPalette = (key) => {
    const root = document.documentElement;
    if (key === "default") {
        root.removeAttribute("data-palette");
    } else {
        root.dataset.palette = key;
    }
    localStorage.setItem("nim-palette", key);
};