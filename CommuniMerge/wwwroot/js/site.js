﻿"use strict";

let chathub = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();
let friendhub = new signalR.HubConnectionBuilder().withUrl("/friendHub").build();

chathub.on("ReceiveMessage", function (receiverUsername, senderUsername, message, sentAt) {

    let newMessage = createMessageHtml(senderUsername, sentAt, message);

    updateLatestMessageListingPersonal(receiverUsername, senderUsername, message);

    addMessageToChatIfActivePersonal(senderUsername, receiverUsername, newMessage);

    playAudio();
});
function updateLatestMessageListingPersonal(receiverUsername, currentUsername, latestMessage) {

    const chatIdentifier1 = `${currentUsername}/${receiverUsername}`;
    const chatIdentifier2 = `${receiverUsername}/${currentUsername}`;
    const list = document.querySelector("#user-list");
    const listItem = list.querySelector(`li[data-chat="${chatIdentifier1}"], li[data-chat="${chatIdentifier2}"]`);
    if (listItem) {
        let messageContent = listItem.querySelector('.truncate');
        if (messageContent) {
            messageContent.textContent = latestMessage;
        }
        else {
            let informationHolder = listItem.querySelector(".message-information");
            messageContent = document.createElement("p");
            messageContent.textContent = latestMessage;
            messageContent.classList.add("truncate");
            informationHolder.appendChild(messageContent);
        }
        list.prepend(listItem);
    }
}
function updateLatestMessageListingGroup(groupId, latestMessage) {
    const list = document.querySelector("#user-list");
    const listItem = list.querySelector(`li[data-group-id="${groupId}"]`);
    if (listItem) {
        let messageContent = listItem.querySelector('.truncate');
        if (messageContent) {
            messageContent.textContent = latestMessage;
        }
        else {
            let informationHolder = listItem.querySelector(".message-information");
            messageContent = document.createElement("p");
            messageContent.textContent = latestMessage;
            messageContent.classList.add("truncate");
            informationHolder.appendChild(messageContent);
        }
        list.prepend(listItem);
    }
}

chathub.on("ReceiveGroupMessage", function (groupId, senderUsername, message, sentAt) {

    let newMessage = createMessageHtml(senderUsername, sentAt, message);

    updateLatestMessageListingGroup(groupId, message);

    addMessageToChatIfActiveGroup(groupId, newMessage);

    playAudio();
});

function addMessageToChatIfActiveGroup(groupId, message) {

    let messageHolder = document.querySelector("#messages");
    let messageSender = document.querySelector("#message-sender");

    let activeReceiver = messageSender.getAttribute("data-receiver");
    if (activeReceiver != groupId) {
        return;
    }

    let firstChild = messageHolder.firstChild;
    if (firstChild != null) {
        messageHolder.insertBefore(message, firstChild);
    }
    else {
        messageHolder.appendChild(message);
    }

}

function addMessageToChatIfActivePersonal(senderUsername, receiverUsername, message) {
    let messageHolder = document.querySelector("#messages");
    let messageSender = document.querySelector("#message-sender");

    const chatIdentifier1 = `${senderUsername}/${receiverUsername}`;
    const chatIdentifier2 = `${receiverUsername}/${senderUsername}`;

    let activeReceiver = messageSender.getAttribute("data-receiver");
    let activeSender = messageSender.getAttribute("data-sender");
    let activeChatIdentifier = activeReceiver + "/" + activeSender;
    if (activeChatIdentifier != chatIdentifier1 && activeChatIdentifier != chatIdentifier2) {
        return;
    }

    let firstChild = messageHolder.firstChild;
    if (firstChild != null) {
        messageHolder.insertBefore(message, firstChild);
    }
    else {
        messageHolder.appendChild(message);
    }
}
friendhub.on("FailSendingFriendRequest", function (feedbackMessage) {

    let messageHTML = createFeedbackMessage(feedbackMessage, false);
    addMessageToFriendAddMenu(messageHTML);
    addFriendMenuFailStyle();
});
friendhub.on("SuccesSendingFriendRequest", function (feedbackMessage) {
    let messageHTML = createFeedbackMessage(feedbackMessage, true);
    addMessageToFriendAddMenu(messageHTML);
    addFriendMenuSuccesStyle();
});

function addFriendMenuFailStyle() {
    let friendRequestInput = document.querySelector("#friendRequestInput");
    friendRequestInput.style.border = "2px solid var(--red)";
}
function addFriendMenuSuccesStyle() {
    let friendRequestInput = document.querySelector("#friendRequestInput");
    friendRequestInput.value = "";
    friendRequestInput.style.border = "2px solid var(--green)";
}
function addMessageToFriendAddMenu(messageHTML) {
    let holder = document.querySelector("#add-friend-input-holder");
    let existingP = holder.querySelector("p");
    if (existingP) {
        existingP.remove();
    }
    holder.prepend(messageHTML);
}
function createFeedbackMessage(feedbackMessage, isSucces) {
    let message = document.createElement("p");
    message.textContent = feedbackMessage;
    if (isSucces) {
        message.classList.add("succes");
    }
    else {
        message.classList.add("fail");
    }
    return message;
}




chathub.start().then(function () {
    console.log("Connected to the server succesfully");
}).catch(function (err) {
    return console.error(err.toString());
});

friendhub.start().then(function () {
    console.log("Connected to the server succesfully");
}).catch(function (err) {
    return console.error(err.toString());
})

friendhub.on("DeleteFriendRequestListing", function (username) {
    deleteFriendRequestOfListing(username);
});

function deleteFriendRequestOfListing(username) {
    let ul = document.querySelector("#friendRequest-list");
    var listItem = ul.querySelector('li[data-user="' + username + '"]');
    if (listItem) {
        listItem.remove();
    } else {
        console.error("Friend request not found.");
    }
}

friendhub.on("ReceiveFriendRequest", function(sender){

    let friendRequestList = document.querySelector("#friendRequest-list");

    let friendRequestListItem = createLiFriendRequestHTML(sender);

    friendRequestList.appendChild(friendRequestListItem);
});
function createLiFriendRequestHTML(sender) {
    let friendRequestListItem = document.createElement("li");
    friendRequestListItem.classList.add("friendRequest-list-item");
    friendRequestListItem.setAttribute("data-user", sender);
    friendRequestListItem.innerHTML= `               
        <p>${sender}</p>
        <div class="friend-request-option-holder">
            <button class="friend-request-option accept" onclick="acceptFriendRequest('${sender}')"><i class="fa-solid fa-check"></i></button>
            <button class="friend-request-option decline" onclick="declineFriendRequest('${sender}')"><i class="fa-solid fa-xmark"></i></button>
        </div>`;
    return friendRequestListItem;
}
function createFriendListItemHTML(currentUser, friend) {
    let li = document.createElement("li");
    li.classList.add("message-item");
    li.setAttribute("data-chat", `${friend}/${currentUser}`);
    li.onclick = function () { openPersonalConversation(this, friend) };
    li.innerHTML =    `
            <header class="message-profile-header">
                <figure class="message-profile user1">
                    <img src="/img/profile.jpg" alt="" srcset="" draggable="false">
                </figure>
                <figure class="message-profile user2">
                    <img src="/img/doggy.jpg" alt="" srcset="" draggable="false">
                </figure>
            </header>
            <div class="message-information">
                <h3>${currentUser} & ${friend}</h3>
            </div>
        `;
    return li;
}

friendhub.on("UpdateFriendListing", function (currentUsersname, friendUsername) {
    let friendListItem = createFriendListItemHTML(currentUsersname, friendUsername);
    addListItemToFriendListing(friendListItem);
});
function addListItemToFriendListing(listItem) {
    let friendListing = document.querySelector("#user-list");
    friendListing.prepend(listItem);
}

function acceptFriendRequest(username){
    friendhub.invoke("AcceptFriendRequest", username);
}

function declineFriendRequest(username){ 
    friendhub.invoke("DeclineFriendRequest", username);
}



document.querySelector("#friendRequestInput").addEventListener("keyup", resetFriendRequestHolder);
function resetFriendRequestHolder() {
    let holder = document.querySelector("#add-friend-input-holder");
    let p = holder.querySelector("p");
    if (p) {
        p.remove();
    }
    let input = holder.querySelector("#friendRequestInput");
    input.style.border = "none";
}
document.getElementById("message-sender").addEventListener("keyup", function (event) {
    let input = document.getElementById("message-sender");
    let message = input.value;
    if (event.key != "Enter" || message.length == 0) {
        return;
    }
    input.value = "";

    let messageSender = document.querySelector("#message-sender");
    let receiver = messageSender.dataset.receiver;

    let isForGroup = messageSender.dataset.forGroup;

    if (isForGroup === "true") {
        chathub.invoke("SendGroupMessage", parseInt(receiver), message).catch(function (err) {
            return console.error(err.toString());
        });
    }
    else if (isForGroup === "false") {
        chathub.invoke("SendMessage", receiver, message).catch(function (err) {
            return console.error(err.toString());
        });
    }

    event.preventDefault();
});

let sendFriendRequestBtn = document.querySelector("#sendFriendRequestBtn");
sendFriendRequestBtn.addEventListener("click", () =>{
    let friendRequestInput = document.querySelector("#friendRequestInput")
    let receiverUsername = friendRequestInput.value;

    friendhub.invoke("SendFriendRequest", receiverUsername).catch(function (err){
        return console.error(err.toString());
    })
    
});


function hideScreenBlocker(){
    let screenBlocker = document.querySelector("#screenBlocker");
    screenBlocker.style.display = "none";
}

function playAudio(){
    let audio = new Audio();
    audio.src = "./Audio/mixkit-long-pop-2358.wav";
    audio.play();
}

async function openGroupConversation(event, group) {
    hideScreenBlocker();
    selectTab(event);

    updateInfoHeader(group.Name);

    updateReceiver(group.Id, true);

    clearMessageHolder();

    let messages = await getGroupMessages(group.Id);
    if (messages != null) {
        addMessagesToMessageHolder(messages);
    }
}

async function getGroupMessages(groupId) {
    const url = `https://localhost:7129/api/Message/getGroup/${groupId}`;
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


async function openPersonalConversation(event, username) {
    hideScreenBlocker();
    selectTab(event);

    updateInfoHeader(username);

    updateReceiver(username, false);

    clearMessageHolder();

    let messages = await getPersonalMessages(username);
    if (messages != null) {
        addMessagesToMessageHolder(messages);
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

function updateReceiver(receiverId, isForGroup){
    let sender = document.querySelector("#message-sender");

    if(!sender.hasAttribute("data-receiver")){
        sender.setAttribute("data-receiver", "default value");
    }
    if (isForGroup) {
        if (!sender.hasAttribute("data-forGroup")) {
            sender.setAttribute("data-forGroup", "default value");
        }
    }

    sender.dataset.forGroup = isForGroup;
    sender.dataset.receiver = receiverId;
}

function updateInfoHeader(name) {
    let infoHeader = document.querySelector("#info-header");
    let headerUsername = document.querySelector("#info-header-username");
    headerUsername.textContent = name;
}

function clearInfoHeader() {
    let infoHeader = document.querySelector("#info-header");
    infoHeader.innerHTML = "";
}


function createMessageHtml(senderUsername, timeStamp, message){
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
        <h3>${senderUsername}</h3>
        <p>${timeStamp}</p>
        </div>
        <div class="message-text">${messageWithLinks}</div>
        </div>
        `;
    return newMessage;
}

function addMessagesToMessageHolder(messageObjects) {
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
async function getPersonalMessages(username) {

    const url = `https://localhost:7129/api/Message/get/${username}`;
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




document.querySelector("#open-friendAddMenu").addEventListener("click", () =>{
    clearFriendsMenu();
    openFriendAddMenu();
});

function openFriendAddMenu(){
    let addFriendMenu = document.querySelector("#add-friend-menu");
    addFriendMenu.style.display = "flex";
}
function clearFriendsMenu(){
    let friendsOverview = document.querySelector("#friendsOverView");
    let addFriendMenu = document.querySelector("#add-friend-menu");
    let friendRequestMenu = document.querySelector("#friendRequestOverView");

    friendRequestMenu.style.display = "none";
    friendsOverview.style.display = "none";
    addFriendMenu.style.display = "none";

}

function openFriendsOverviewMenu(){
    let friendsOverview = document.querySelector("#friendsOverView");
    friendsOverview.style.display = "flex";
}

function openFriendsMenu(){
    let menu = document.querySelector("#menu-background");
    clearFriendsMenu();
    openFriendsOverviewMenu();
    menu.style.display = "block";
}


function hideMenu(){
    document.querySelector("#menu-background").style.display = "none";
}

document.getElementById('menu-popup').addEventListener('click', function(event) {
    event.stopPropagation();
});


function openFriendRequestMenu(){

    clearFriendsMenu();
    let menu = document.querySelector("#friendRequestOverView");

    menu.style.display = "flex";
    
}

function apendChatLoadingScreen(){
    let messageList = document.querySelector("#messages");

    for(let i = 0; i < 20; ++i){
        let li = document.createElement("li");
        
        
    }
}