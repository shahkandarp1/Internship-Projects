const changeMode = () => {
    try {   
        const mode = localStorage.getItem("mode")
        if(mode==null || mode=="Light"){
            localStorage.setItem("mode","Dark")
            document.getElementById("body").style.backgroundColor = "black";
            document.getElementById("moon").classList.add("hidden")
            document.getElementById("sun").classList.remove("hidden")
            document.getElementById("bottom-flex").style.color = "white"
            document.getElementsByClassName("bottom-part-a")[0].style.color = "white"
            document.getElementsByClassName("bottom-part-a")[1].style.color = "white"
        }
        else{
            localStorage.setItem("mode","Light")
            document.getElementById("body").style.backgroundColor = "rgba(250, 250, 250, 1)";
            document.getElementById("sun").classList.add("hidden")
            document.getElementById("moon").classList.remove("hidden")
            document.getElementById("bottom-flex").style.color = "#3E3E3E"
            document.getElementsByClassName("bottom-part-a")[0].style.color = "#3E3E3E"
            document.getElementsByClassName("bottom-part-a")[1].style.color = "#3E3E3E"
        }
    } catch (err) {
        alert("there was some issue in changing mode")
    }
}

const checkMode = () => {
    const mode = localStorage?.getItem("mode")
    console.log(mode)
    try {   
        if(mode=="Light" || mode==null){
            console.log("in try")
            document.getElementById("body").style.backgroundColor = "rgba(250, 250, 250, 1)";
            document.getElementById("sun").classList.add("hidden")
            document.getElementById("moon").classList.remove("hidden")
            document.getElementById("bottom-flex").style.color = "#3E3E3E"
            document.getElementsByClassName("bottom-part-a")[0].style.color = "#3E3E3E"
            document.getElementsByClassName("bottom-part-a")[1].style.color = "#3E3E3E"
        }
        else{
            document.getElementById("body").style.backgroundColor = "black";
            document.getElementById("moon").classList.add("hidden")
            document.getElementById("sun").classList.remove("hidden")
            document.getElementById("bottom-flex").style.color = "white"
            document.getElementsByClassName("bottom-part-a")[0].style.color = "white"
            document.getElementsByClassName("bottom-part-a")[1].style.color = "white"
        }
    } catch (err) {
        console.log(err)
        alert("there was some issue in checking mode")
    }
}