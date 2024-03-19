var mode = localStorage.getItem('mode') || 'light';
var html = document.documentElement; // Reference to <html> element
var btn = document.getElementById('pg-theme-btn');

var butn = document.getElementById('theme-btn');
var main = document.getElementById('mainn');

// Set the initial theme based on the mode retrieved from local storage
setTheme(mode);

btn.addEventListener("click", toggleTheme);

function toggleTheme() {
    mode = mode === "light" ? "dark" : "light"; // Toggle mode
    setTheme(mode);
    localStorage.setItem('mode', mode); // Store the theme preference in local storage
}

function setTheme(mode) {
    html.setAttribute('data-bs-theme', mode);
    if (mode === "light") {
        butn.classList.add('bi-moon');
        butn.classList.remove('bi-sun');
        document.getElementById('nav-name-text').style.color = "#3E3E3E";
        document.getElementById('nav-mob-name-text').style.color = "#3E3E3E";
        btn.style.backgroundColor = "white";
        if (main) {
            main.style.backgroundColor = "#FAFAFA";
        }
    } else {
        butn.classList.remove('bi-moon');
        butn.classList.add('bi-sun');
        document.getElementById('nav-name-text').style.color = "white";
        document.getElementById('nav-mob-name-text').style.color = "white";
        btn.style.backgroundColor = "black";
        if (main) {
            main.style.backgroundColor = "#6c757d";
        }
    }
}