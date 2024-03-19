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
function sidebar_close() {
    document.getElementById("mySidebar").style.width = "0px";
    document.getElementsByClassName("overlay")[0].style.display = "none";
    for (let i = 0; i < document.getElementsByClassName('accordion-button').length; ++i) {
        document.getElementsByClassName('accordion-button')[i].style.zIndex = "1";
    }
}
$(document).ready(function () {
    $("#openBtn").click(function () {
        $("#myModal1").modal("show");
        var Street = $("#street").val();
        var City = $("#city").val();
        var State = $("#state").val();
        var ZipCode = $("#zipcode").val();
        var address = "https://maps.google.com/maps?q=" + Street + City + State + ZipCode + "&t=&z=13&ie=UTF8&iwloc=&output=embed";
        $("#gmap_canvas").attr("src", address);
    });
});


const Edit = () => {
    const editable = document.getElementsByClassName('editable')
    for (let i = 0; i < editable.length; ++i) {
        editable[i].disabled = false;
    }
    const btn = document.getElementsByClassName('lower-custom-btn')
    for (let i = 0; i < btn.length; ++i) {
        if (btn[i].classList.length == 3) {
            btn[i].classList.remove("hidden");
        }
        else {
            btn[i].classList.add("hidden");
        }
    }
}

const Cancel = () => {
    location.reload()
}

$(document).ready(function () {
    $("#closeBtn1").click(function () {
        $("#myModal1").modal("hide");
    });
});

const changeMode = () => {
    try {
        const mode = localStorage.getItem("mode")
        if (mode == null || mode == "light") {
            localStorage.setItem("mode", "dark")
            document.getElementById("body").style.backgroundColor = "black";
            document.getElementById("submit-text").style.color = "white"
            document.getElementById("moon").classList.add("hidden")
            document.getElementById("sun").classList.remove("hidden")
            document.getElementsByClassName("main-form")[0].style.backgroundColor = "rgba(173, 173, 173, 0.8)"
        }
        else {
            localStorage.setItem("mode", "light")
            document.getElementById("body").style.backgroundColor = "#FAFAFA";
            document.getElementById("submit-text").style.color = "black"
            document.getElementById("sun").classList.add("hidden")
            document.getElementById("moon").classList.remove("hidden")
            document.getElementsByClassName("main-form")[0].style.backgroundColor = "white"
        }
    } catch (err) {
        alert("there was some issue in changing mode")
    }
}

window.onload = function () {

    const inputDate = document.getElementById('dob');
    const today = new Date().toISOString().split('T')[0];
    inputDate.setAttribute('max', today);

    //used in encounter form

    const inputDatee = document.getElementById('date');
    if (inputDatee) {
        const todayy = new Date().toISOString().split('T')[0];
        inputDatee.setAttribute('max', todayy);
    }

    const phoneInputField = document.getElementsByClassName("phone");
    for (let i = 0; i < phoneInputField.length; ++i) {
        const phoneInput = window.intlTelInput(phoneInputField[i], {
            utilsScript:
                "https://cdnjs.cloudflare.com/ajax/libs/intl-tel-input/17.0.8/js/utils.js",
        });
    }

    const mode = localStorage.getItem("mode")
    if (mode == "light" || mode == null) {
        document.getElementById("body").style.backgroundColor = "#FAFAFA";
        document.getElementById("submit-text").style.color = "black"
        document.getElementById("sun").classList.add("hidden")
        document.getElementById("moon").classList.remove("hidden")
        document.getElementsByClassName("main-form")[0].style.backgroundColor = "white"
    }
    else {
        document.getElementById("body").style.backgroundColor = "black";
        document.getElementById("submit-text").style.color = "white"
        document.getElementById("moon").classList.add("hidden")
        document.getElementById("sun").classList.remove("hidden")
        document.getElementsByClassName("main-form")[0].style.backgroundColor = "rgba(173, 173, 173, 0.8)"
    }
}
