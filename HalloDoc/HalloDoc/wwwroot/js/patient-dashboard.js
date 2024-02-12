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

const changeMode = () => {
  try {
    const mode = localStorage.getItem("mode")
    if (mode == null || mode == "Light") {
      localStorage.setItem("mode", "Dark")
      document.getElementById("body").style.backgroundColor = "black";
      document.getElementById("main-heading").style.color = "white"
      document.getElementById("moon").classList.add("hidden")
      document.getElementById("sun").classList.remove("hidden")
      document.getElementsByClassName("main-content")[0].style.backgroundColor = "rgba(173, 173, 173, 0.8)"
    }
    else {
      localStorage.setItem("mode", "Light")
      document.getElementById("body").style.backgroundColor = "#FAFAFA";
      document.getElementById("main-heading").style.color = "black"
      document.getElementById("sun").classList.add("hidden")
      document.getElementById("moon").classList.remove("hidden")
      document.getElementsByClassName("main-content")[0].style.backgroundColor = "white"
    }
  } catch (err) {
    alert("there was some issue in changing mode")
  }
}

const toggleButton = (curr_btn,redirect_page) => {
  const buttons = document.getElementsByClassName("common-btn")
  for (let i = 0; i < buttons.length; ++i) {
    if (buttons[i] != curr_btn) {
      buttons[i].classList.remove('create-request-active')
    }
    else {
      buttons[i].classList.add('create-request-active')
    }
  }
  document.getElementById("redirect-value").value = redirect_page;
}

const redirect = () => {
    window.location.assign(`./${document.getElementById("redirect-value").value}`);
}

$(document).ready(function () {
    var logElements = document.querySelectorAll('.log');
    logElements.forEach(function (element) {
        element.addEventListener('click', function () {

            $.get('/Patient/Logout', function (response) {
                console.log(response)
            });
        });
    });
});

window.onload = (e) => {

        const mode = localStorage.getItem("mode")
        if (mode == "Light" || mode == null) {
            document.getElementById("body").style.backgroundColor = "#FAFAFA";
            document.getElementById("main-heading").style.color = "black"
            document.getElementById("sun").classList.add("hidden")
            document.getElementById("moon").classList.remove("hidden")
            document.getElementsByClassName("main-content")[0].style.backgroundColor = "white"
        }
        else {
            document.getElementById("body").style.backgroundColor = "black";
            document.getElementById("main-heading").style.color = "white"
            document.getElementById("moon").classList.add("hidden")
            document.getElementById("sun").classList.remove("hidden")
            document.getElementsByClassName("main-content")[0].style.backgroundColor = "rgba(173, 173, 173, 0.8)"
        }
    
}