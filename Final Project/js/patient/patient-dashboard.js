function sidebar_open() {
    document.getElementById("mySidebar").style.width = "200px";
    document.getElementsByClassName("overlay")[0].style.display = "block";
    for(let i=0;i<document.getElementsByClassName('accordion-button').length;++i){
        document.getElementsByClassName('accordion-button')[i].style.zIndex = "-1";
    }
  }
  
  function sidebar_close() {
    document.getElementById("mySidebar").style.width = "0px";
    document.getElementsByClassName("overlay")[0].style.display = "none";
    for(let i=0;i<document.getElementsByClassName('accordion-button').length;++i){
        document.getElementsByClassName('accordion-button')[i].style.zIndex = "1";
    }
  }