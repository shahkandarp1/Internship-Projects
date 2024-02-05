


const changeMode = () => {
    try {
        const mode = localStorage.getItem("mode")
        if (mode == null || mode == "Light") {
            localStorage.setItem("mode", "Dark")
            document.getElementById("body").style.backgroundColor = "black";
            document.getElementById("i-am-text").style.color = "white"
            document.getElementById("moon").classList.add("hidden")
            document.getElementById("sun").classList.remove("hidden")
        }
        else {
            localStorage.setItem("mode", "Light")
            document.getElementById("body").style.backgroundColor = "#FAFAFA";
            document.getElementById("i-am-text").style.color = "black"
            document.getElementById("sun").classList.add("hidden")
            document.getElementById("moon").classList.remove("hidden")
        }
    } catch (err) {
        alert("there was some issue in changing mode")
    }
}


window.onload = () => {
    
        const mode = localStorage.getItem("mode")
        if (mode == "Light" || mode == null) {
            document.getElementById("body").style.backgroundColor = "#FAFAFA";
            document.getElementById("i-am-text").style.color = "black"
            document.getElementById("sun").classList.add("hidden")
            document.getElementById("moon").classList.remove("hidden")
        }
        else {
            document.getElementById("body").style.backgroundColor = "black";
            document.getElementById("i-am-text").style.color = "white"
            document.getElementById("moon").classList.add("hidden")
            document.getElementById("sun").classList.remove("hidden")

        }
    
}