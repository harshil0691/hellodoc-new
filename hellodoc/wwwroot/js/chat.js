"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

//Disable the send button until connection is established.
document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (message, action) {

    var chatdiv = document.createElement("div");
    var chat = document.createElement("div");
    var timeParent = document.createElement("span");
    var timechild = document.createElement("span");

    chatdiv.classList.add("message", "received", "mx-2");
    timeParent.classList.add("metadata");
    timechild.classList.add("time");

    chatdiv.appendChild(chat);
    chatdiv.appendChild(timeParent);
    timeParent.appendChild(timechild);

    document.getElementById("messagesList").appendChild(chatdiv);

    chat.textContent = `${message}`;
    const now = new Date();
    const hours = now.getHours();
    const minutes = now.getMinutes();
    const ampm = hours >= 12 ? "PM" : "AM";
    const twelveHourHours = hours % 12 || 12;
    const formattedMinutes = minutes < 10 ? "0" + minutes : minutes;

    timechild.textContent = `${twelveHourHours}:${formattedMinutes} ${ampm}`;
});

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    debugger;
    return console.error(err.toString());
});

function sendMessage(requestid) {
    var user = document.getElementById("userInput").value;
    var message = document.getElementById("messageInput").value;
    var SenderAspid = document.getElementById("SenderAspid").value;
    var ReciverAspid = document.getElementById("ReciverAspid").value;
    var sentFrom = document.getElementById("sentFrom").value;

    document.getElementById("messageInput").value = "";
    connection.invoke("SendMessage", user, message, SenderAspid, ReciverAspid, sentFrom, requestid).then(function () {

        var chatdiv = document.createElement("div");
        var chat = document.createElement("div");
        var timeParent = document.createElement("span");
        var timechild = document.createElement("span");

        chatdiv.classList.add("message", "sent", "mx-2");
        timeParent.classList.add("metadata");
        timechild.classList.add("time");

        chatdiv.appendChild(chat);
        chatdiv.appendChild(timeParent);
        timeParent.appendChild(timechild);

        document.getElementById("messagesList").appendChild(chatdiv);

        chat.textContent = `${message}`;
        const now = new Date();
        const hours = now.getHours();
        const minutes = now.getMinutes();
        const ampm = hours >= 12 ? "PM" : "AM";
        const twelveHourHours = hours % 12 || 12;
        const formattedMinutes = minutes < 10 ? "0" + minutes : minutes;

        timechild.textContent = `${twelveHourHours}:${formattedMinutes} ${ampm}`;

    }).catch(function (err) {
        return console.error(err.toString());
    });

    event.preventDefault();
}