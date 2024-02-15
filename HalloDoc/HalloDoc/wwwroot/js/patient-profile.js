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
$("#openBtn").click(function () {
    $("#myModal1").modal("show");
    var geocoder = new google.maps.Geocoder();
    var address = `${document.getElementById("street").value}, ${document.getElementById("city").value},${document.getElementById("state").value} ${document.getElementById("zipcode").value}`
    geocoder.geocode({ address: address }, function (results, status) {
        if (status == "OK") {
            var latitude = results[0].geometry.location.lat();
            var longitude = results[0].geometry.location.lng();
            console.log("Latitude: " + latitude);
            console.log("Longitude: " + longitude);
            var map = L.map('map').setView([latitude, longitude], 13);
            L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
                attribution: 'Map data © <a href="https://www.openstreetmap.org/">OpenStreetMap</a> contributors',
                maxZoom: 18
            }).addTo(map);
            var marker = L.marker([latitude, longitude]).addTo(map);
            marker.bindPopup("This is Your Location").openPopup();
        } else {
            console.log("Geocoding failed: " + status);
        }
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
    const editable = document.getElementsByClassName('editable')
    for (let i = 0; i < editable.length; ++i) {
        editable[i].disabled = true;
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

$(document).ready(function () {
    $("#closeBtn1").click(function () {
        $("#myModal1").modal("hide");
    });
});

const changeMode = () => {
    try {
        const mode = localStorage.getItem("mode")
        if (mode == null || mode == "Light") {
            localStorage.setItem("mode", "Dark")
            document.getElementById("body").style.backgroundColor = "black";
            document.getElementById("submit-text").style.color = "white"
            document.getElementById("moon").classList.add("hidden")
            document.getElementById("sun").classList.remove("hidden")
            document.getElementsByClassName("main-form")[0].style.backgroundColor = "rgba(173, 173, 173, 0.8)"
        }
        else {
            localStorage.setItem("mode", "Light")
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

    const phoneInputField = document.getElementsByClassName("phone");
    for (let i = 0; i < phoneInputField.length; ++i) {
        const phoneInput = window.intlTelInput(phoneInputField[i], {
            utilsScript:
                "https://cdnjs.cloudflare.com/ajax/libs/intl-tel-input/17.0.8/js/utils.js",
        });
    }

    const mode = localStorage.getItem("mode")
    if (mode == "Light" || mode == null) {
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

