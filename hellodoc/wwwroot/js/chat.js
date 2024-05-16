"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

//Disable the send button until connection is established.
document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (user, message) {
    var li = document.createElement("li");
    document.getElementById("messagesList").appendChild(li);
    // We can assign user-supplied strings to an element's textContent because it
    // is not interpreted as markup. If you're assigning in any other way, you 
    // should be aware of possible script injection concerns.
    li.textContent = `${user} says ${message}`;
});

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

function sendMessage(requestid) {
    var user = document.getElementById("userInput").value;
    var message = document.getElementById("messageInput").value;
    var SenderAspid = document.getElementById("SenderAspid").value;
    var ReciverAspid = document.getElementById("ReciverAspid").value;
    var sentFrom = document.getElementById("sentFrom").value;

    document.getElementById("messageInput").value = "";
    connection.invoke("SendMessage", user, message, SenderAspid, ReciverAspid, sentFrom, requestid).catch(function (err) {
        return console.error(err.toString());
    });

    event.preventDefault();
}