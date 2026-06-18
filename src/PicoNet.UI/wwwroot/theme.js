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