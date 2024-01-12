var i = 0
let first = 0
if(Number(window.innerWidth) <= 1300 && first==0){
  i=1;
  first=1
}
function openNav() {
  console.log(window.innerWidth)
  if(Number(window.innerWidth) <= 1300 && first==0){
    i=1;
    first=1
  }
  if(i==0){
    i=1;
    document.getElementById("mySidebar").style.width = "0px";
    // document.getElementById("main").style.marginLeft = "0px";
  }
  else{
    i=0
    document.getElementById("mySidebar").style.width = "200px";
    // document.getElementById("main").style.marginLeft = "200px";
    
  }
}

/* Set the width of the sidebar to 0 and the left margin of the page content to 0 */
// function closeNav() {
//   document.getElementById("mySidebar").style.width = "0";
//   document.getElementById("main").style.marginLeft = "0";
// }