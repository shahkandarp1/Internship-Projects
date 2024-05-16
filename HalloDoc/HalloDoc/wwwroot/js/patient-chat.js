//Chat code
var connection = new signalR.HubConnectionBuilder().withUrl("/chathub").build();

connection.on("ReceiveMessage", function (reciever, requestId, message, timestamp) {
    const container = document.getElementById(`Provider-${requestId}-${reciever}`);
    const newChatMessage = document.createElement("div");
    newChatMessage.classList.add("chat-message");
    newChatMessage.classList.add("recieved");
    newChatMessage.innerHTML = `
              <div class="message-box">
                <p class="message-content">${message}</p>
              </div>
              <p class="message-time">${timestamp}</p>
            `;
    container.appendChild(newChatMessage);
});

connection.on("ReceiveMessageInGroup", function (reciever, requestId, message, timestamp, chatwith, sentby) {
    const container = document.getElementById(`Admin-${requestId}`);
    const newChatMessage = document.createElement("div");
    newChatMessage.classList.add("chat-message");
    if (sentby == "Admin") {
        newChatMessage.classList.add("recieved");
    }
    newChatMessage.innerHTML = `
              <div class="message-box">
                <p class="message-content">${message}</p>
              </div>
              <p class="message-time">${timestamp}</p>
            `;
    container.appendChild(newChatMessage);
});

connection.start()

var chat = document.getElementsByClassName("chat-btn")
for (let i = 0; i < chat.length; ++i) {
    chat[i].onclick = function () {
        var reqid = chat[i].getAttribute("data-reqid");
        var aspid = chat[i].getAttribute("data-aspid");
        document.getElementById("req-id").value = reqid;
        document.getElementById("asp-id").value = aspid;
    };
}