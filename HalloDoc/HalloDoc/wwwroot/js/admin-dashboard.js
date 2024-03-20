const changeStatus = (element) => {

    designStatus(element.classList[1])
    let current_state = element.classList[1] 
    let name = current_state.split("-").length == 1 ? current_state.split("-")[0] : current_state.split("-")[1]

    console.log(name.charAt(0).toUpperCase() + name.slice(1))

    const state = document.getElementsByClassName("req-filter")
    for (let i = 0; i < state.length; ++i) {
        state[i].classList.remove(`filter-active`)
    }
    state[0].classList.add(`filter-active`)

    document.getElementById("search").value = ""

    localStorage.setItem("status", name.charAt(0).toUpperCase() + name.slice(1))
    localStorage.setItem("search", "")
    localStorage.setItem("requestor", "All")
    localStorage.setItem("region", -1)
    localStorage.setItem("page", 1)
    localStorage.setItem("pageSize", 10)

    document.getElementById("region").value = -1
    document.getElementsByClassName("loader-container")[0].style.display = "flex";
    document.getElementById("partial-container").style.display = "none";
    $.ajax({
        url: `/Admin/${name.charAt(0).toUpperCase() + name.slice(1)}`,
        type: 'POST',
        dataType: 'html',
        // Pass your data here
        success: function (response) {
            document.getElementsByClassName("loader-container")[0].style.display = "none";
            document.getElementById("partial-container").style.display = "block";
            $("#partial-container").html(response)
        },
        error: function () {
            alert('An error occurred.');
        }
    });

}

const designStatus = (namee) => {
    const states = document.getElementsByClassName("row-cards")
    const current_state = namee
    for (let i = 0; i < states.length; ++i) {
        if (states[i].classList.length == 3 && states[i].classList[1] == current_state) {
            break;
        }
        else if (states[i].classList[1] == current_state) {
            states[i].classList.add(`${states[i].classList[1]}-active`)
            document.getElementById(`${states[i].classList[1]}-active`).classList.remove('hidden')
            document.getElementById(`${states[i].classList[1]}`).classList.add('hidden')
            document.getElementById(`${states[i].classList[1]}-img`).classList.remove('hidden')
        }
        else if (states[i].classList.length == 3) {
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

    designRequestor(element.classList[1])
    filter(1,10)

}

const designRequestor = (name) => {
    const states = document.getElementsByClassName("req-filter")
    const current_state = name

    for (let i = 0; i < states.length; ++i) {
        if (states[i].classList.length == 3 && states[i].classList[1] == current_state) {
            break;
        }
        else if (states[i].classList[1] == current_state) {
            states[i].classList.add(`filter-active`)
        }
        else if (states[i].classList.length == 3) {
            states[i].classList.remove(`filter-active`)
        }
    }
}
