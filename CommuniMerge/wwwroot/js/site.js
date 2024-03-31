"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

connection.on("ReceiveMessage", function (user, message, sentAt) {
    var newMessage = document.createElement("li");
    newMessage.classList.add("message-wrapper");

    let urlRegex = /(https?:\/\/[^\s]+)/g;
    let messageWithLinks = message.replace(urlRegex, (url) =>{
        return '<a href="' + url + '" target="_blank">' + url + '</a>';
    });
    newMessage.innerHTML = `                    
    <figure class="message-profile-picture">
        <img src="/img/profile.jpg">
    </figure>
    <div class="content-wrapper">
        <div class="identifier-wrapper">
            <h3>${user}</h3>
            <p>${sentAt}</p>
        </div>
        <div class="message-text">${messageWithLinks}</div>
    </div>
    `;
    let messageHolder = document.querySelector("#messages");
    let firstChild = messageHolder.firstChild;
    if(firstChild != null){
        messageHolder.insertBefore(newMessage, firstChild);
    }
    else{
        messageHolder.appendChild(newMessage);
    }

});

connection.start().then(function () {
    console.log("Connected to the server succesfully");
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("message-sender").addEventListener("keyup", function (event) {
    if(event.key != "Enter"){
        return;
    }
    let input = document.getElementById("message-sender");
    var user = "TEST";
    var message = input.value;
    input.value = "";
    connection.invoke("SendMessage", user, message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});

function clearMessageHolder(){
    let messageHolder = document.querySelector("#messages");
    messageHolder.innerHTML = "";
}

function addMessagesToMessageHolder(){

}



let messageDisplays = document.querySelectorAll(".message-item");

messageDisplays.forEach((messageDisplay) =>{
    messageDisplay.addEventListener("onclick", (event) =>{
        openConversation(event);
    });
});




function openConversation(event){
    clearMessageHolder();
    addMessagesToMessageHolder();
}