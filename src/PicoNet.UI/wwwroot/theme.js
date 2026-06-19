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

window.nimTheme = {
    setMode: (mode) => {
        const root = document.documentElement;
        root.classList.toggle("dark", mode === "dark");
        localStorage.setItem("nim-mode", mode);
    },
    setPalette: (palette) => {
        document.documentElement.dataset.palette = palette;
        localStorage.setItem("nim-palette", palette);
    },
    init: () => {
        const mode = localStorage.getItem("nim-mode") || "light";
        const palette = localStorage.getItem("nim-palette") || "nim-indigo";
        document.documentElement.classList.toggle("dark", mode === "dark");
        document.documentElement.dataset.palette = palette;
    }
};