

const changeVisibility = () => {
    var x = document.getElementById("floatingPassword");
    const openeye = document.getElementsByClassName("open-eye")
    const closeeye = document.getElementsByClassName("close-eye")
    if (x.type === "password") {
        x.type = "text";
        for(let i=0;i<openeye.length;++i){
            openeye[i].classList.add("hidden")
            closeeye[i].classList.remove("hidden")
        }
    } else {
        x.type = "password";
        for(let i=0;i<openeye.length;++i){
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

const changeMode = () => {
    try {   
        const mode = localStorage.getItem("mode")
        let formtext = document.getElementsByClassName("form-label")
        if(mode==null || mode=="light"){
            localStorage.setItem("mode","dark")
            document.getElementById("body").style.backgroundColor = "rgba(62, 62, 62, 0.5)";
            document.getElementById("login-text").style.color = "white"
            document.getElementById("moon").classList.add("hidden")
            document.getElementById("sun").classList.remove("hidden")
            document.getElementById("bottom-flex").style.color = "white"
            document.getElementsByClassName("bottom-part-a")[0].style.color = "white"
            document.getElementsByClassName("bottom-part-a")[1].style.color = "white"
            for(let i=0;i<formtext.length;++i){
                formtext[i].style.color = "white"
            }
        }
        else{
            localStorage.setItem("mode","light")
            document.getElementById("body").style.backgroundColor = "transparent";
            document.getElementById("login-text").style.color = "#3e3e3e"
            document.getElementById("sun").classList.add("hidden")
            document.getElementById("moon").classList.remove("hidden")
            document.getElementById("bottom-flex").style.color = "#3E3E3E"
            document.getElementsByClassName("bottom-part-a")[0].style.color = "#3E3E3E"
            document.getElementsByClassName("bottom-part-a")[1].style.color = "#3E3E3E"
            for(let i=0;i<formtext.length;++i){
                formtext[i].style.color = "#7D787E"
            }
        }
    } catch (err) {
        alert("there was some issue in changing mode")
    }
}


window.onload = () => {
    const mode = localStorage?.getItem("mode")
    let formtext = document.getElementsByClassName("form-label")
    
        if (mode == "light" || mode == null) {
            console.log("in try")
            document.getElementById("body").style.backgroundColor = "transparent";
            document.getElementById("login-text").style.color = "#3e3e3e"
            document.getElementById("sun").classList.add("hidden")
            document.getElementById("moon").classList.remove("hidden")
            document.getElementById("bottom-flex").style.color = "#3E3E3E"
            document.getElementsByClassName("bottom-part-a")[0].style.color = "#3E3E3E"
            document.getElementsByClassName("bottom-part-a")[1].style.color = "#3E3E3E"
            for (let i = 0; i < formtext.length; ++i) {
                formtext[i].style.color = "#7D787E"
            }
        }
        else {
            document.getElementById("body").style.backgroundColor = "rgba(62, 62, 62, 0.5)";
            document.getElementById("login-text").style.color = "white"
            document.getElementById("moon").classList.add("hidden")
            document.getElementById("sun").classList.remove("hidden")
            document.getElementById("bottom-flex").style.color = "white"
            document.getElementsByClassName("bottom-part-a")[0].style.color = "white"
            document.getElementsByClassName("bottom-part-a")[1].style.color = "white"
            for (let i = 0; i < formtext.length; ++i) {
                formtext[i].style.color = "white"
            }
        }
    
}