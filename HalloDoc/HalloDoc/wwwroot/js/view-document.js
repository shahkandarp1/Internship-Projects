const getFileData = (myFile) => {
    var file = myFile.files[0];
    var filename = file.name;
    document.getElementById("form-label").innerHTML = `${filename}`;
}

function sidebar_open() {
    if (document.getElementById("mySidebar").offsetWidth == 0) {
        document.getElementById("mySidebar").style.width = "200px";
        document.getElementsByClassName("overlay")[0].style.display = "block";
        document.getElementsByClassName("navbar")[0].style.boxShadow = "none";
    }
    else {
        document.getElementById("mySidebar").style.width = "0px";
        document.getElementsByClassName("overlay")[0].style.display = "none";
        document.getElementsByClassName("navbar")[0].style.boxShadow = "3px -8px 17px 1px black";
    }

}

$(document).ready(function () {
    var logElements = document.querySelectorAll('.log');
    logElements.forEach(function (element) {
        element.addEventListener('click', function () {

            $.get('/Patient/Logout', function (response) {
                console.log(response)
            });
        });
    });
});

function sidebar_close() {
    document.getElementById("mySidebar").style.width = "0px";
    document.getElementsByClassName("overlay")[0].style.display = "none";
    for (let i = 0; i < document.getElementsByClassName('accordion-button').length; ++i) {
        document.getElementsByClassName('accordion-button')[i].style.zIndex = "1";
    }
}


const tickAll = () => {
    const checkboxes = document.getElementsByClassName("checkbox")
    if (document.getElementsByClassName("checkbox-main")[0].checked == true) {
        for (let i = 0; i < checkboxes.length; ++i) {
            checkboxes[i].checked = true;
        }
    }
    else {
        for (let i = 0; i < checkboxes.length; ++i) {
            checkboxes[i].checked = false;
        }
    }
} 

const allCheck = () => {
    const checkboxes = document.getElementsByClassName("checkbox")
    let flag = 0
    for (let i = 0; i < checkboxes.length; ++i) {
        if (checkboxes[i].checked == false) {
            flag = 1
            break
        }
    }
    if (flag == 0) {
        document.getElementsByClassName("checkbox-main")[0].checked = true;
    }
    else {
        document.getElementsByClassName("checkbox-main")[0].checked = false;
    }
}

const changeMode = () => {
    
        const mode = localStorage.getItem("mode")
        if (mode == null || mode == "Light") {
            localStorage.setItem("mode", "Dark")
            document.getElementById("body").style.backgroundColor = "black";
            document.getElementById("moon").classList.add("hidden")
            document.getElementById("sun").classList.remove("hidden")
            document.getElementsByClassName("main-content")[0].style.backgroundColor = "rgba(173, 173, 173, 0.8)"
            for (let i = 0; i < document.getElementsByClassName('dark-text').length; ++i) {
                document.getElementsByClassName('dark-text')[i].style.color = "white";
            }
            for (let i = 0; i < document.getElementsByClassName('dark-heading-text').length; ++i) {
                document.getElementsByClassName('dark-heading-text')[i].style.color = "white";
            }
        }
        else {
            localStorage.setItem("mode", "Light")
            document.getElementById("body").style.backgroundColor = "#FAFAFA";
            document.getElementById("sun").classList.add("hidden")
            document.getElementById("moon").classList.remove("hidden")
            document.getElementsByClassName("main-content")[0].style.backgroundColor = "white"
            for (let i = 0; i < document.getElementsByClassName('dark-text').length; ++i) {
                document.getElementsByClassName('dark-text')[i].style.color = "rgb(150,150,150)";
            }
            for (let i = 0; i < document.getElementsByClassName('dark-heading-text').length; ++i) {
                document.getElementsByClassName('dark-heading-text')[i].style.color = "#3E3E3E";
            }
        }
    
}

window.onload = (e) => {

    const mode = localStorage.getItem("mode")
    if (mode == "Light" || mode == null) {
        localStorage.setItem("mode", "Light")
        document.getElementById("body").style.backgroundColor = "#FAFAFA";
        document.getElementById("sun").classList.add("hidden")
        document.getElementById("moon").classList.remove("hidden")
        document.getElementsByClassName("main-content")[0].style.backgroundColor = "white"
        for (let i = 0; i < document.getElementsByClassName('dark-text').length; ++i) {
            document.getElementsByClassName('dark-text')[i].style.color = "rgb(150,150,150)";
        }
        for (let i = 0; i < document.getElementsByClassName('dark-heading-text').length; ++i) {
            document.getElementsByClassName('dark-heading-text')[i].style.color = "#3E3E3E";
        }
    }
    else {
        localStorage.setItem("mode", "Dark")
        document.getElementById("body").style.backgroundColor = "black";
        document.getElementById("moon").classList.add("hidden")
        document.getElementById("sun").classList.remove("hidden")
        document.getElementsByClassName("main-content")[0].style.backgroundColor = "rgba(173, 173, 173, 0.8)"
        for (let i = 0; i < document.getElementsByClassName('dark-text').length; ++i) {
            document.getElementsByClassName('dark-text')[i].style.color = "white";
        }
        for (let i = 0; i < document.getElementsByClassName('dark-heading-text').length; ++i) {
            document.getElementsByClassName('dark-heading-text')[i].style.color = "white";
        }
    }

}