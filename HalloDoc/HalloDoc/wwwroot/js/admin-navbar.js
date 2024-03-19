function sidebar_open() {
    if (document.getElementById("mySidebar").offsetWidth == 0) {
        document.getElementById("mySidebar").style.width = "200px";
        document.getElementsByClassName("overlay")[0].style.display = "block";
        /*document.getElementsByClassName("navbar")[0].style.boxShadow = "none";*/
    }
    else {
        document.getElementById("mySidebar").style.width = "0px";
        document.getElementsByClassName("overlay")[0].style.display = "none";
        /*document.getElementsByClassName("navbar")[0].style.boxShadow = "3px -8px 17px 1px black";*/
    }

}

function sidebar_close() {
    document.getElementById("mySidebar").style.width = "0px";
    document.getElementsByClassName("overlay")[0].style.display = "none";
    for (let i = 0; i < document.getElementsByClassName('accordion-button').length; ++i) {
        document.getElementsByClassName('accordion-button')[i].style.zIndex = "1";
    }
}

const navbar = (element) => {
    const navbar = document.getElementsByClassName("navbar-btn")
    for (let i = 0; i < navbar.length; ++i) {
        navbar[i].classList.remove("sidebar-active")
    }

    element.classList.add("sidebar-active")

    localStorage.removeItem("status");
    localStorage.removeItem("requestor");
    localStorage.removeItem("page");
    localStorage.removeItem("pageSize");
    localStorage.removeItem("search");
    localStorage.removeItem("region");
}

var acc = document.getElementsByClassName("accordion-mob-btn");
var i;

for (i = 0; i < acc.length; i++) {
    acc[i].addEventListener("click", function () {
        this.classList.toggle("accord-active");
        var panel = this.nextElementSibling;
        if (panel.style.display === "block") {
            panel.style.display = "none";
        } else {
            panel.style.display = "block";
        }
    });
}

const clearStorage = () => {
    localStorage.removeItem("status");
    localStorage.removeItem("requestor");
    localStorage.removeItem("page");
    localStorage.removeItem("pageSize");
    localStorage.removeItem("search");
    localStorage.removeItem("region");
}