"use strict";

let chathub = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();
let friendhub = new signalR.HubConnectionBuilder().withUrl("/friendHub").build();

chathub.on("ReceiveMessage", function (receiverUsername, senderUsername, message, sentAt) {

    let newMessage = createMessageHtml(senderUsername, sentAt, message);

    updateLatestMessageListing(receiverUsername, senderUsername, message);

    addMessageToChatIfActive(senderUsername, receiverUsername, newMessage);

    playAudio();
});

function addMessageToChatIfActive(senderUsername, receiverUsername, message) {
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

function updateLatestMessageListing(receiverUsername, currentUsername, latestMessage) {

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
    li.onclick = function () { openConversation(this, friend) };
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





document.getElementById("message-sender").addEventListener("keyup", function (event) {
    let input = document.getElementById("message-sender");
    let message = input.value;
    if (event.key != "Enter" || message.length == 0) {
        return;
    }
    input.value = "";

    let receiver = document.querySelector("#message-sender").dataset.receiver;
    


    chathub.invoke("SendMessage", receiver, message).catch(function (err) {
        return console.error(err.toString());
    });
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

async function openConversation(event, username) {
    hideScreenBlocker();
    selectTab(event);

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
    let headerUsername = document.querySelector("#info-header-username");
    headerUsername.textContent = username;
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