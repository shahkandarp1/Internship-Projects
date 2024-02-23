
window.onload = function () {

    const inputDate = document.getElementById('dob');
    const today = new Date().toISOString().split('T')[0];
    inputDate.setAttribute('max', today);

    const phoneInputField = document.getElementsByClassName("phone");
    for (let i = 0; i < phoneInputField.length; ++i) {
        const phoneInput = window.intlTelInput(phoneInputField[i], {
            utilsScript:
                "https://cdnjs.cloudflare.com/ajax/libs/intl-tel-input/17.0.8/js/utils.js",
        });
    }
}