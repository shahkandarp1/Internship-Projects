window.onload = function () {
    const phoneInputField = document.querySelector("#phone");
    const phoneInput = window.intlTelInput(phoneInputField, {
        utilsScript:
            "https://cdnjs.cloudflare.com/ajax/libs/intl-tel-input/17.0.8/js/utils.js",
    });
}

const getFileData = (myFile) => {
    var file = myFile.files[0];  
    var filename = file.name;
    document.getElementById("form-label").innerHTML = `${filename}`;
}

const changeMode = () => {
    try {   
        const mode = localStorage.getItem("mode")
        if(mode==null || mode=="Light"){
            localStorage.setItem("mode","Dark")
            document.getElementById("body").style.backgroundColor = "black";
            document.getElementById("submit-text").style.color = "white"
            document.getElementById("moon").classList.add("hidden")
            document.getElementById("sun").classList.remove("hidden")
        }
        else{
            localStorage.setItem("mode","Light")
            document.getElementById("body").style.backgroundColor = "#FAFAFA";
            document.getElementById("submit-text").style.color = "black"
            document.getElementById("sun").classList.add("hidden")
            document.getElementById("moon").classList.remove("hidden")
        }
    } catch (err) {
        alert("there was some issue in changing mode")
    }
}

const checkMode = (e) => {
    try {   
        const mode = localStorage.getItem("mode")
        if(mode=="Light" || mode==null){
            document.getElementById("body").style.backgroundColor = "#FAFAFA";
            document.getElementById("submit-text").style.color = "black"
            document.getElementById("sun").classList.add("hidden")
            document.getElementById("moon").classList.remove("hidden")
        }
        else{
            document.getElementById("body").style.backgroundColor = "black";
            document.getElementById("submit-text").style.color = "white"
            document.getElementById("moon").classList.add("hidden")
            document.getElementById("sun").classList.remove("hidden")
            
        }
    } catch (err) {
        alert("there was some issue in changing mode")
    }
}

const reload = () => {
    location.reload()
}