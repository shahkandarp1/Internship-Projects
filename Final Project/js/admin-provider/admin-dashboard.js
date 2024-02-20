const  changeStatus = (element) => {
    const states = document.getElementsByClassName("row-cards")
    const current_state = element.classList[1]
    for(let i=0;i<states.length;++i){
        if(states[i].classList.length == 3 && states[i].classList[1] == current_state){
            break;
        }
        else if(states[i].classList[1] == current_state){
            states[i].classList.add(`${states[i].classList[1]}-active`)
            document.getElementById(`${states[i].classList[1]}-active`).classList.remove('hidden')
            document.getElementById(`${states[i].classList[1]}`).classList.add('hidden')
            document.getElementById(`${states[i].classList[1]}-img`).classList.remove('hidden')
        }
        else if(states[i].classList.length == 3)
        {
            states[i].classList.remove(`${states[i].classList[1]}-active`)
            document.getElementById(`${states[i].classList[1]}-active`).classList.add('hidden')
            document.getElementById(`${states[i].classList[1]}`).classList.remove('hidden')
            document.getElementById(`${states[i].classList[1]}-img`).classList.add('hidden')
        }
    }
    let name = current_state.split("-").length == 1 ? current_state.split("-")[0] : current_state.split("-")[1]
    document.getElementById("status-text").innerHTML = `(${name.charAt(0).toUpperCase() + name.slice(1)})`
}

const changeRequestor = (element) => {
    const states = document.getElementsByClassName("req-filter")
    const current_state = element.classList[1]
    for(let i=0;i<states.length;++i){
        if(states[i].classList.length == 3 && states[i].classList[1] == current_state){
            break;
        }
        else if(states[i].classList[1] == current_state){
            states[i].classList.add(`filter-active`)
        }
        else if(states[i].classList.length == 3)
        {
            states[i].classList.remove(`filter-active`)
        }
    }
}