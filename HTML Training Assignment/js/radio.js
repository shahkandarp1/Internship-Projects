
let i=0

const changeImage = ()=>{
  const img_div = document.querySelectorAll(".check-icon")
  if(i==0){
    i=1
    for(let j=0;j<img_div.length;++j){
      console.log(img_div[j])
      img_div[j].src="./image/checked-icon.svg"
      console.log(img_div[j].src)
    }
    
  }
  else{
    i=0
    for(let j=0;j<img_div.length;++j){
      console.log(img_div)
      img_div[j].src="./image/check-icon.svg"
    }
    
  }
}