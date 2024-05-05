"use strict";

let chathub = new signalR.HubConnectionBuilder().withUrl("https://localhost:7129/chatHub").build();
let friendhub = new signalR.HubConnectionBuilder().withUrl("https://localhost:7129/friendHub").build();

chathub.on("ReceiveMessage", function (receiverUsername, messageDto) {
    let senderDto = messageDto.sender;
    messageDto.timeStamp = messageDto.timeStamp.substring(0, 16).toString().replace("T", " ");

    let newMessage = createMessageHtml(messageDto);
    updateLatestMessageListingPersonal(receiverUsername, messageDto.sender.username, messageDto.content);

    addMessageToChatIfActivePersonal(senderDto.username, receiverUsername, newMessage);

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

chathub.on("ReceiveGroupMessage", function (groupId, messageDto) {

    messageDto.timeStamp = messageDto.timeStamp.substring(0, 16).toString().replace("T", " ");

    let newMessage = createMessageHtml(messageDto);

    updateLatestMessageListingGroup(groupId, messageDto.content);

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

friendhub.on("ReceiveFriendRequest", function (friendRequestDto) {

    let sender = friendRequestDto.sender;

    let friendRequestList = document.querySelector("#friendRequest-list");

    let friendRequestListItem = createLiFriendRequestHTML(sender.username);

    friendRequestList.appendChild(friendRequestListItem);
});
function createLiFriendRequestHTML(sender) {
    let friendRequestListItem = document.createElement("li");
    friendRequestListItem.classList.add("friendRequest-list-item");
    friendRequestListItem.setAttribute("data-user", sender);
    friendRequestListItem.innerHTML = `               
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
    li.innerHTML = `
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

friendhub.on("UpdateFriendListing", function (currentUser, friend) {
    let friendListItem = createFriendListItemHTML(currentUser.username, friend.username);
    addListItemToFriendListing(friendListItem);
});
function addListItemToFriendListing(listItem) {
    let friendListing = document.querySelector("#user-list");
    friendListing.prepend(listItem);
}

function acceptFriendRequest(username) {
    const url = `https://localhost:7129/api/User/acceptFriendRequest/${username}`;
    fetch(url, {
        method: 'POST',
        credentials: "include",
        body: null
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            console.log('Message sent successfully');
        })
        .catch(error => {
            console.error('There was a problem with the request:', error);
        });
}

function declineFriendRequest(username) {
    const url = `https://localhost:7129/api/User/declineFriendRequest/${username}`;
    fetch(url, {
        method: 'POST',
        credentials: "include",
        body: null
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            console.log('Message sent successfully');
        })
        .catch(error => {
            console.error('There was a problem with the request:', error);
        });
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

    let fileInput = document.querySelector("#message-file-selector");

    let isForGroup = messageSender.dataset.forGroup;

    if (isForGroup === "true") {
        let groupMessageCreateDto = {
            groupId: receiver,
            content: message,
            file: null,
        }

        if (fileInput.files.length > 0) {
            groupMessageCreateDto.file = fileInput.files[0];
        }

        let formData = new FormData();
        formData.append('groupId', groupMessageCreateDto.groupId);
        formData.append('content', groupMessageCreateDto.content);
        if (groupMessageCreateDto.file) {
            formData.append('file', groupMessageCreateDto.file);
        }

        fetch('https://localhost:7129/api/Message/CreateGroupMessage', {
            method: 'POST',
            credentials: "include",
            body: formData
        })
            .then(response => {
                if (!response.ok) {
                    throw new Error('Network response was not ok');
                }
                console.log('Message sent successfully');
            })
            .catch(error => {
                console.error('There was a problem with the request:', error);
            });

    }
    else if (isForGroup === "false") {
        let personalMessageCreateDto = {
            receiverUsername: receiver,
            content: message,
            file: null
        }
        if (fileInput.files.length > 0) {
            personalMessageCreateDto.file = fileInput.files[0];
        }

        let formData = new FormData();
        formData.append('ReceiverUsername', personalMessageCreateDto.receiverUsername);
        formData.append('content', personalMessageCreateDto.content);
        if (personalMessageCreateDto.file) {
            formData.append('file', personalMessageCreateDto.file);
        }

        fetch('https://localhost:7129/api/Message/createPersonalMessage', {
            method: 'POST',
            credentials: "include",
            body: formData
        })
            .then(response => {
                if (!response.ok) {
                    throw new Error('Network response was not ok');
                }
                console.log('Message sent successfully');
            })
            .catch(error => {
                console.error('There was a problem with the request:', error);
            });

    }

    event.preventDefault();
});

let sendFriendRequestBtn = document.querySelector("#sendFriendRequestBtn");
sendFriendRequestBtn.addEventListener("click", () => {
    let friendRequestInput = document.querySelector("#friendRequestInput")
    let receiverUsername = friendRequestInput.value;


    const url = `https://localhost:7129/api/User/sendFriendRequest/${receiverUsername}`;
    fetch(url, {
        method: 'POST',
        credentials: "include",
        body: null
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            console.log('Message sent successfully');
        })
        .catch(error => {
            console.error('There was a problem with the request:', error);
        });

});


function hideScreenBlocker() {
    let screenBlocker = document.querySelector("#screenBlocker");
    screenBlocker.style.display = "none";
}

function playAudio() {
    let audio = new Audio();
    audio.src = "./Audio/mixkit-long-pop-2358.wav";
    audio.play();
}

async function openGroupConversation(event, group) {
    hideScreenBlocker();
    selectTab(event);

    updateInfoHeader(group.Name, group.ProfilePath);

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


        data.forEach(d => {
            d.timeStamp = d.timeStamp.substring(0, 16).toString().replace("T", " ");
        });

        return data;
    } catch (error) {
        console.error('Error fetching messages:', error.message);
        return null;
    }
}


async function openPersonalConversation(event, userDto) {
    hideScreenBlocker();
    selectTab(event);

    updateInfoHeader(userDto.Username, userDto.ProfilePath);

    updateReceiver(userDto.Username, false);

    clearMessageHolder();

    let messages = await getPersonalMessages(userDto.Username);
    if (messages != null) {
        addMessagesToMessageHolder(messages);
    }
}

function selectTab(event) {

    var messageItems = document.querySelectorAll(".message-item");
    var user2s = document.querySelectorAll(".user2");

    user2s.forEach(user =>{
        user.style.outline = "2px solid var(--white)";
    });

    for (let i = 0; i < messageItems.length; ++i) {
        let messageItem = messageItems[i];
        messageItem.style.backgroundColor = "var(--white)";
    }
    event.style.backgroundColor = "var(--light-blue)";
    let user = event.querySelector(".user2");
    if(user != null){
        user.style.outline = "2px solid var(--light-blue)";
    }
}

function updateReceiver(receiverId, isForGroup) {
    let sender = document.querySelector("#message-sender");

    if (!sender.hasAttribute("data-receiver")) {
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

function updateInfoHeader(name, profilePath) {
    let infoHeader = document.querySelector("#info-header");
    let headerUsername = document.querySelector("#info-header-username");
    let img = document.querySelector("#pfp-img");
    img.src = profilePath;
    headerUsername.textContent = name;
}

function clearInfoHeader() {
    let infoHeader = document.querySelector("#info-header");
    infoHeader.innerHTML = "";
}


function createMessageHtml(messageObject) {
    let newMessage = document.createElement("li");
    newMessage.classList.add("message-wrapper");

    let urlRegex = /(https?:\/\/[^\s]+)/g;
    let messageWithLinks = messageObject.content.replace(urlRegex, (url) => {
        return '<a href="' + url + '" target="_blank">' + url + '</a>';
    });

    if (messageObject.fileType == null) {

        newMessage.innerHTML = `                    
            <figure class="message-profile-picture">
                <img src="${messageObject.sender.profilePath}">
            </figure>
            <div class="content-wrapper">
                <div class="identifier-wrapper">
                    <h3>${messageObject.sender.username}</h3>
                    <p>${messageObject.timeStamp}</p>
                </div>
                <div class="message-text">${messageWithLinks}</div>
            </div>
            `;
    }
    else if (messageObject.fileType == 0) {
        newMessage.innerHTML = `                    
            <figure class="message-profile-picture">
                <img src="${messageObject.sender.profilePath}">
            </figure>
            <div class="content-wrapper">
                <div class="identifier-wrapper">
                    <h3>${messageObject.sender.username}</h3>
                    <p>${messageObject.timeStamp}</p>
                </div>
                <figure class="message-image-holder" >
                    <img src="${messageObject.filePath}" onclick="popUpImage(this)" />
                </figure>
                <div class="message-text">${messageWithLinks}</div>
            </div>
            `;

    }
    else if (messageObject.fileType == 1) {
        newMessage.innerHTML = `                    
            <figure class="message-profile-picture">
                <img src="${messageObject.sender.profilePath}">
            </figure>
            <div class="content-wrapper">
                <div class="identifier-wrapper">
                    <h3>${messageObject.sender.username}</h3>
                    <p>${messageObject.timeStamp}</p>
                </div>
                <video class="message-video-holder" type="video/mp4" controls>
                    <source src="${messageObject.filePath}" />
                </video>
                <div class="message-text">${messageWithLinks}</div>
            </div>
            `;
    }
    return newMessage;
}

function popUpImage(imageTag){
    let imagePath = imageTag.src;
    let div = document.createElement("div");

    let img = document.createElement("img");
    img.src = imagePath;
    div.id = "image-popup";

    div.innerHTML = `
        <button id="closeImagePopup" onclick="deletePopupImage()"><i class="fa-solid fa-xmark"></i></button>

        <figure class="image-of-popup">

        </figure>

    `;

    let figure = div.querySelector(".image-of-popup");

    img.addEventListener("load", function(){
        figure.appendChild(img);
        let body = document.body;
        body.addEventListener("keyup", escapeDelete);
        body.appendChild(div);
    });

}

function escapeDelete(event) {
    if(event.key === "Escape") {
        deletePopupImage();
        document.body.removeEventListener("keyup", escapeDelete);
    }
}

function deletePopupImage(){

    document.querySelector("#image-popup").remove();
}

function addMessagesToMessageHolder(messageObjects) {
    let messageHolder = document.querySelector("#messages");

    for (let i = 0; i < messageObjects.length; ++i) {

        let messageObject = messageObjects[i];

        let newMessage = createMessageHtml(messageObject);

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

        data.forEach(d => {
            d.timeStamp = d.timeStamp.substring(0, 16).toString().replace("T", " ");
        });

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




function openFriendAddMenu() {
    clearMenu();
    setMenuHeader("Add friends");
    let addFriendMenu = document.querySelector("#add-friend-menu");
    addFriendMenu.style.display = "flex";
};


function clearMenu() {
    setMenuHeader("");
    let menus = document.querySelector("#menu-popup").querySelectorAll(".menu-body");

    menus.forEach(menu => {
        menu.style.display = "none";
    });
}

function openFriendsOverviewMenu() {
    let friendsOverview = document.querySelector("#friendsOverView");
    friendsOverview.style.display = "flex";
}

function openFriendsMenu() {
    clearMenu();
    setMenuHeader("Friends");
    openFriendsOverviewMenu();
    showMenu();
}

function setMenuHeader(value) {
    document.querySelector("#menu-header-title").textContent = value;
}
function clearMenuHeader() {
    document.querySelector("#menu-header-title").textContent = "";
}

function showMenu() {
    let menu = document.querySelector("#menu-background");
    menu.style.display = "block";

}
function hideMenu() {
    document.querySelector("#menu-background").style.display = "none";
    clearMenu();
    clearMenuHeader();
}

document.getElementById('menu-popup').addEventListener('click', function (event) {
    event.stopPropagation();
});


function openFriendRequestMenu() {
    setMenuHeader("Friend requests");
    clearMenu();
    let menu = document.querySelector("#friendRequestOverView");

    menu.style.display = "flex";

}

function apendChatLoadingScreen() {
    let messageList = document.querySelector("#messages");

    for (let i = 0; i < 20; ++i) {
        let li = document.createElement("li");


    }
}


async function openGroupCreationMenu() {
    clearMenu();
    var groupMenu = document.querySelector("#groupCreate");
    groupMenu.style.display = "flex";

    clearFriendsForGroup();
    let friends = await getFriends();

    addFriendsToGroupCreateFriendList(friends);


    setMenuHeader("Select friends for group");
    showMenu();
}

async function getFriends() {
    const url = `https://localhost:7129/api/User/friends`;
    try {
        const response = await fetch(url, {
            method: 'GET',
            credentials: "include"

        });

        if (!response.ok) {
            throw new Error(`Failed to fetch messages. Status: ${response.status}`);
        }

        const friends = await response.json();

        return friends;
    } catch (error) {
        console.error('Error fetching messages:', error.message);
        return null;
    }
}

function addFriendsToGroupCreateFriendList(friends) {
    let ul = document.querySelector("#group-add-friends-list");
    friends.forEach(friend => {
        let li = document.createElement("li");
        li.classList.add("group-add-list-item");

        li.innerHTML = `
        <p>${friend.username}</p>
        `
        ul.prepend(li);
    });

}

function clearFriendsForGroup() {
    let list = document.querySelector("#group-add-friends-list");
    list.innerHTML = "";
}
document.querySelector("#open-file-group-create-btn").addEventListener("click", function () {
    document.querySelector("#group-image-input").click();
});
document.querySelector("#add-file-btn").addEventListener("click", function () {
    document.querySelector("#message-file-selector").click();
});





function openGroupSettingsMenu() {

    clearMenu();

    let infoMenu = document.querySelector("#groupInfoCreate");
    infoMenu.style.display = "flex";



    setMenuHeader("Group settings");
    showMenu();
}

function openProfileMenu() {




    showMenu();
}

function showGroupCreationMenu() {
    clearMenu();
    setMenuHeader("Select friends for group");
    let groupCreate = document.querySelector("#groupCreate");
    groupCreate.style.display = "flex";
}

function hideGroupCreationMenu() {
    let groupCreate = document.querySelector("#groupCreate");
    groupCreate.style.display = "none";
}

function openGroupCreateInfoForm() {
    hideGroupCreationMenu();





}
function openSettingsMenu() {
    clearMenu();


    showMenu();
}