const getFileData = (myFile) => {
    var file = myFile.files[0];
    var filename = file.name;
    document.getElementById("form-label").innerHTML = `${filename}`;
}


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

const reload = () => {
    location.reload()
}

$(window).on('load', function () {
    $('#myModal').modal('show');
});

$(document).ready(function () {
    $("#closeBtn").click(function () {
        $("#myModal").modal("hide");
    });
});

$(document).ready(function () {
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
});

$(document).ready(function () {
    $("#closeBtn1").click(function () {
        $("#myModal1").modal("hide");
    });
});


const changeVisibility = () => {
    var x = document.getElementById("floatingPassword");
    const openeye = document.getElementsByClassName("open-eye")
    const closeeye = document.getElementsByClassName("close-eye")
    if (x.type === "password") {
        x.type = "text";
        for (let i = 0; i < openeye.length; ++i) {
            openeye[i].classList.add("hidden")
            closeeye[i].classList.remove("hidden")
        }
    } else {
        x.type = "password";
        for (let i = 0; i < openeye.length; ++i) {
            openeye[i].classList.remove("hidden")
            closeeye[i].classList.add("hidden")
        }
    }
}

const changeVisibility1 = () => {
    var x = document.getElementById("floatingConfirmPassword");
    if (x.type === "password") {
        x.type = "text";
        document.getElementById("open-eye").classList.add("hidden")
        document.getElementById("closed-eye").classList.remove("hidden")
    } else {
        x.type = "password";
        document.getElementById("open-eye").classList.remove("hidden")
        document.getElementById("closed-eye").classList.add("hidden")
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
