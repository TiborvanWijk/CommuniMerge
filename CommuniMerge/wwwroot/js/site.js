"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

connection.on("ReceiveMessage", function (user, message, sentAt) {

    let newMessage = createMessageHtml(user, sentAt, message);

    let messageHolder = document.querySelector("#messages");
    let firstChild = messageHolder.firstChild;
    if (firstChild != null) {
        messageHolder.insertBefore(newMessage, firstChild);
    }
    else {
        messageHolder.appendChild(newMessage);
    }

});

connection.start().then(function () {
    console.log("Connected to the server succesfully");
}).catch(function (err) {
    return console.error(err.toString());
});


connection.on("ReceiveFriendRequest", function(sender){
    console.log(sender);
});

document.getElementById("message-sender").addEventListener("keyup", function (event) {
    let input = document.getElementById("message-sender");
    let message = input.value;
    if (event.key != "Enter" || message.length == 0) {
        return;
    }
    input.value = "";

    let receiver = document.querySelector("#message-sender").dataset.receiver;
    


    connection.invoke("SendMessage", receiver, message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});

let sendFriendRequestBtn = document.querySelector("#sendFriendRequestBtn");
sendFriendRequestBtn.addEventListener("click", () =>{
    let friendRequestInput = document.querySelector("#friendRequestInput")
    let receiverUsername = friendRequestInput.value;

    connection.invoke("SendFriendRequest", receiverUsername).catch(function (err){
        return console.error(err.toString());
    })
    
});


let messageDisplays = document.querySelectorAll(".message-item");

messageDisplays.forEach((messageDisplay) => {
    messageDisplay.addEventListener("onclick", (event) => {
        openConversation(event);
    });
});




async function openConversation(event, username) {
    selectTab(event);
    clearInfoHeader();

    updateInfoHeader(username);

    updateReceiver(username);

    clearMessageHolder();

    let messages = await getMessages(username);
    if (messages != null) {
        addMessagesToMessageHolder(messages, username);
    }
}

function selectTab(event){
    
    var messageItems = document.querySelectorAll(".message-item");

    for(let i = 0; i < messageItems.length; ++i){
        let messageItem = messageItems[i];
        messageItem.style.backgroundColor = "var(--white)";
    }
    event.style.backgroundColor = "var(--light-blue)";
}

function updateReceiver(username){
    let sender = document.querySelector("#message-sender");

    if(!sender.hasAttribute("data-receiver")){
        sender.setAttribute("data-receiver", "default value");
    }

    sender.dataset.receiver = username;
}

function updateInfoHeader(username) {
    let infoHeader = document.querySelector("#info-header");
    infoHeader.innerHTML = `
            <div class="userinfo-wrapper">
                <figure class="user-pfp">
                    <img src="/img/profile.jpg" />
                </figure>
                <div class="userinfo">
                    <h3>${username}</h3>
                </div>
            </div>

            <ul class="message-menu">
                <li></li>
                <li></li>
                <li></li>
            </ul>`;
}

function clearInfoHeader() {
    let infoHeader = document.querySelector("#info-header");
    infoHeader.innerHTML = "";
}


function createMessageHtml(username, timeStamp, message){
    let newMessage = document.createElement("li");
        newMessage.classList.add("message-wrapper");

        let urlRegex = /(https?:\/\/[^\s]+)/g;
        let messageWithLinks = message.replace(urlRegex, (url) => {
            return '<a href="' + url + '" target="_blank">' + url + '</a>';
        });
        newMessage.innerHTML = `                    
        <figure class="message-profile-picture">
        <img src="/img/profile.jpg">
        </figure>
        <div class="content-wrapper">
        <div class="identifier-wrapper">
        <h3>${username}</h3>
        <p>${timeStamp}</p>
        </div>
        <div class="message-text">${messageWithLinks}</div>
        </div>
        `;
    return newMessage;
}

function addMessagesToMessageHolder(messageObjects, username) {
    let messageHolder = document.querySelector("#messages");

    for (let i = 0; i < messageObjects.length; ++i) {

        let messageObject = messageObjects[i];

        let newMessage = createMessageHtml(messageObject.senderUsername, messageObject.timeStamp, messageObject.content);

        let firstChild = messageHolder.firstChild;
        if (firstChild != null) {
            messageHolder.insertBefore(newMessage, firstChild);
        }
        else {
            messageHolder.appendChild(newMessage);
        }
    }
}
async function getMessages(username) {

    const url = `https://localhost:7129/get/${username}`;
    try {
        const response = await fetch(url, {
            method: 'GET',
            credentials: "include"

        });

        if (!response.ok) {
            throw new Error(`Failed to fetch messages. Status: ${response.status}`);
        }

        const data = await response.json();

        return data;
    } catch (error) {
        console.error('Error fetching messages:', error.message);
        return null;
    }
}

function clearMessageHolder() {
    let messageHolder = document.querySelector("#messages");
    messageHolder.innerHTML = "";
}


function getCookie(cookieName) {
    const cookies = document.cookie;

    for (let i = 0; i < cookies.length; i++) {
        const cookie = cookies[i];
        if (cookie.startsWith(cookieName)) {
            return cookie;
        }
    }
    return null;
}







function hideMenu(){
    document.querySelector("#menu-background").style.display = "none";
}

document.getElementById('menu-popup').addEventListener('click', function(event) {
    event.stopPropagation();
});

function openFriendAddingMenu(event, username){
    let menuBackground = document.querySelector("#menu-background");
    menuBackground.style.display = "block";
}

function addHeaderToMenu(headerValue){

}