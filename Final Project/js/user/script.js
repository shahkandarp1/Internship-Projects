

const changeVisibility = () => {
    var x = document.getElementById("floatingPassword");
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