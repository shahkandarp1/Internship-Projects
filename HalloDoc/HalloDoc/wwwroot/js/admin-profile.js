
const top_most_edit = () => {
    const editable = document.getElementsByClassName('top-most-editable')
    for (let i = 0; i < editable.length; ++i) {
        editable[i].disabled = false;
    }
    const topbtn = document.getElementById("top-most-edit")
    const btn = topbtn.getElementsByClassName('lower-custom-btn')
    for (let i = 0; i < btn.length; ++i) {
        if (btn[i].classList.length == 3) {
            btn[i].classList.remove("hidden");
        }
        else {
            btn[i].classList.add("hidden");
        }
    }
}


const top_edit = () => {
    const editable = document.getElementsByClassName('top-editable')
    for (let i = 0; i < editable.length; ++i) {
        editable[i].disabled = false;
    }
    const topbtn = document.getElementById("top-edit")
    const btn = topbtn.getElementsByClassName('lower-custom-btn')
    for (let i = 0; i < btn.length; ++i) {
        if (btn[i].classList.length == 3) {
            btn[i].classList.remove("hidden");
        }
        else {
            btn[i].classList.add("hidden");
        }
    }
}

const bottom_edit = () => {
    const editable = document.getElementsByClassName('bottom-editable')
    for (let i = 0; i < editable.length; ++i) {
        editable[i].disabled = false;
    }
    const bottombtn = document.getElementById("bottom-edit")
    const btn = bottombtn.getElementsByClassName('lower-custom-btn')
    for (let i = 0; i < btn.length; ++i) {
        if (btn[i].classList.length == 3) {
            btn[i].classList.remove("hidden");
        }
        else {
            btn[i].classList.add("hidden");
        }
    }
}

const removeDisable = () => {
    const editable = document.getElementsByClassName('editable')
    for (let i = 0; i < editable.length; ++i) {
        editable[i].disabled = false;
    }
}


window.onload = () => {
    const phoneInputField = document.getElementsByClassName("phone");
    for (let i = 0; i < phoneInputField.length; ++i) {
        const phoneInput = window.intlTelInput(phoneInputField[i], {
            utilsScript:
                "https://cdnjs.cloudflare.com/ajax/libs/intl-tel-input/17.0.8/js/utils.js",
        });
    }
}