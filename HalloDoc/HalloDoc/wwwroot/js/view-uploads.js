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