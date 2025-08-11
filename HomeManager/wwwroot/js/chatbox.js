//const currentUserId = document.getElementById("senderId")?.value;
window.startConnection = async function () {
    if (window.connection.state === "Connected" || window.connection.state === "Connecting") {
        console.log("Already connected or connecting.");
        return;
    }

    try {
        await window.connection.start();
        console.log("SignalR connected. State:", window.connection.state);
    } catch (err) {
        console.error("Connection failed, retrying in 5s", err);
        setTimeout(window.startConnection, 5000);
    }
};


window.connection = new signalR.HubConnectionBuilder()
    .withUrl("/hubs/chat")
    .configureLogging(signalR.LogLevel.Information)
    .build();


window.startConnection();

document.addEventListener("DOMContentLoaded", function () {
    const openChatBtn = document.getElementById("openChatBtn");
    const closeChatBtn = document.getElementById("closeChatBtn");
    const chatContainer = document.getElementById("chatPopup");
    const messageInput = document.getElementById("messageInput");
    if (openChatBtn) {
        openChatBtn.addEventListener("click", async () => {
            console.log("Open Chat button clicked");
            const homeId = openChatBtn.dataset.homeId;
            await window.prepareChat(homeId);
        });
    } else {
        console.warn("openChatBtn not found in DOM");
    }

    if (closeChatBtn) {
        closeChatBtn.addEventListener("click", () => {
            chatContainer.style.display = "none";
        });
    } else {
        console.warn("closeChatBtn not found in DOM");
    }

    if (messageInput) {
        messageInput.addEventListener("keydown", function (e) {
            if (e.key === "Enter" && !e.shiftKey) {
                e.preventDefault();
                sendMessage();
            }
        });
    }
});

let chatLoading = false;

window.prepareChat = async function (homeId) {
    if (chatLoading) {
        console.log("Chat is already being prepared. Ignoring extra click.");
        return;
    }

    chatLoading = true;

    try {
        if (window.connection.state !== "Connected") {
            console.log("Connection state is", window.connection.state, ". Waiting for connection...");
            await window.startConnection();
        }

        const res = await fetch(`/Chat/ForHome/${homeId}`, { credentials: 'include' });
        if (!res.ok) {
            const errorText = await res.text();
            alert("Error: " + errorText);
            return;
        }

        const data = await res.json();
        const conversationId = data.conversationId;
        const otherUserName = data.otherUserName;

        document.querySelector("#chatPopup .chat-header span").textContent = "Chat with " + otherUserName;

        await window.connection.invoke("JoinConversationGroup", conversationId);
        console.log("Joined SignalR group for conversation:", conversationId);

        const messagesRes = await fetch(`/Chat/Messages?conversationId=${conversationId}`);
        const messages = await messagesRes.json();

        document.getElementById("conversationId").value = conversationId;
        //document.getElementById("chatContainer").style.display = "block";
        document.getElementById("chatPopup").style.display = "block";

        const list = document.getElementById("messagesList");
        list.innerHTML = "";
        messages.forEach(msg => {
            const isSelf = msg.senderId === document.getElementById("senderId").value;
            const li = document.createElement("div");
            li.classList.add("message", isSelf ? "self" : "other");

            const displayName = isSelf ? "You" : (msg.senderName || "Dealer");
            const statusHtml = isSelf
                ? `<div class="message-status">[${window.getMessageStatusText(msg.status)}]</div>`
                : '';

            const timestamp = new Date(msg.sentAt).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
            li.innerHTML = `
        <div class="message-bubble">
            <div class="message-author">${displayName} <span class="message-timestamp">${timestamp}</span></div>
            <div class="message-content">${msg.content}</div>
            ${statusHtml}
        </div>
    `;
            list.appendChild(li);
        });

        await window.connection.invoke("MarkAsSeen", conversationId, currentUserId);
    } catch (err) {
        console.error("Error in prepareChat:", err);
    } finally {
        chatLoading = false;
    }
};

window.getMessageStatusText = function (messageStatus) {
    const num = parseInt(messageStatus ?? 0);
    switch (num) {
        case 0: return "Pending";
        case 1: return "Sent";
        case 2: return "Delivered";
        case 3: return "Seen";
        case 4: return "Failed";
        default: return `Unknown (${messageStatus})`;
    }
};



function sendMessage() {
    const currentUserId = document.getElementById("senderId")?.value;
    const msg = document.getElementById("messageInput").value;
    const conversationId = document.getElementById("conversationId")?.value;

    if (!msg || !conversationId || !currentUserId) {
        console.error("Missing fields for message sending.");
        return;
    }

    const tempId = "msg_" + Date.now();

    const messageDto = {
        conversationId: conversationId,
        senderId: currentUserId,
        content: msg,
        sentAt: new Date().toISOString(),
        status: 0,
        tempId: tempId
    };

    const li = document.createElement("div");
    li.id = tempId;
    li.dataset.content = msg;
    li.dataset.temp = "true";
    li.classList.add("message", "self");

    li.innerHTML = `
        <div class="message-bubble">
            <div class="message-author">You:</div>
            <div class="message-content">${msg}</div>
            <div class="message-status">
                <span class="status-text status-text-${tempId}" style="font-size:0.8em; color: gray;">[${getMessageStatusText(0)}]</span>
            </div>
        </div>
    `;
    document.getElementById("messagesList").appendChild(li);
    document.getElementById("messagesList").scrollTop = document.getElementById("messagesList").scrollHeight;





    window.connection.invoke("SendMessage", messageDto)
        .then(() => console.log("Message sent"))
        .catch(err => console.error("SignalR SendMessage failed:", err));

    document.getElementById("messageInput").value = "";
}

window.connection.on("ReceiveMessage", async (message) => {
    const currentUserId = document.getElementById("senderId")?.value;
    const isSelf = message.senderId === currentUserId;
    const li = document.createElement("div");
    li.classList.add("message", isSelf ? "self" : "other");
    const displayName = isSelf ? "You" : (message.senderName ? message.senderName : "Dealer");
    if (isSelf && message.tempId) {
        const tempEl = document.getElementById(message.tempId);

        if (tempEl) {
            const statusEl = tempEl.querySelector("span.status-text");
            if (statusEl) {
                statusEl.className = `status-text status-text-${message.messageId}`;
                statusEl.textContent = `[${getMessageStatusText(message.messageStatus)}]`;
            }

            tempEl.dataset.temp = "false";
            tempEl.id = message.messageId;
            return;
        }
    }

    li.innerHTML = `
        <div class="message-bubble">
            <div class="message-author">${displayName}:</div>
            <div class="message-content">${message.content}</div>
            ${isSelf ? `<div class="message-status">[${getMessageStatusText(message.status)}]</div>` : ''}
        </div>
    `;
    document.getElementById("messagesList").appendChild(li);
    document.getElementById("messagesList").scrollTop = document.getElementById("messagesList").scrollHeight;

    if (!isSelf && message.receiverId === currentUserId) {
        try {
            await window.connection.invoke("MarkAsDelivered", message.messageId);
            const convoId = document.getElementById("conversationId")?.value;
            if (convoId) {
                await window.connection.invoke("MarkAsSeen", convoId, currentUserId);
            }
        } catch (err) {
            console.error("Failed to mark as delivered or seen", err);
        }
    }
});

window.connection.on("ReceiveMessageStatusUpdate", (data) => {
    const statusEl = document.querySelector(`.status-text-${data.messageId}`);
    if (statusEl) {
        statusEl.textContent = `[${getMessageStatusText(data.status)}]`;
    }
});

window.connection.onreconnected(async () => {
    const conversationId = document.getElementById("conversationId")?.value;
    if (conversationId) {
        try {
            await window.connection.invoke("JoinConversationGroup", conversationId);
            console.log("Re-joined group after reconnect:", conversationId);
        } catch (err) {
            console.error("Failed to re-join group:", err);
        }
    }
});

window.prepareChatBox = function () {
    const currentUserId = document.getElementById("senderId")?.value;
    const conversationId = document.getElementById("conversationId")?.value;
    document.getElementById("chatPopup").style.display = "block";
    if (window.connection && conversationId) {
        window.connection.invoke("JoinConversationGroup", conversationId);
    }
    fetch(`/Chat/Messages?conversationId=${conversationId}&currentUserId=${currentUserId}`)
        .then(res => res.json())
        .then(messages => {
            const list = document.getElementById("messagesList");
            list.innerHTML = "";
            messages.forEach(msg => {
                const isSelf = msg.senderId === currentUserId;
                const li = document.createElement("div");
                li.classList.add("message", isSelf ? "self" : "other");
                const displayName = isSelf ? "You" : (msg.senderName || "Dealer");
                const statusHtml = isSelf
                    ? `<div class="message-status">[${window.getMessageStatusText(msg.status)}]</div>`
                    : '';
                li.innerHTML = `
                    <div class="message-bubble">
                        <div class="message-author">${displayName}:</div>
                        <div class="message-content">${msg.content}</div>
                        ${statusHtml}
                    </div>
                `;
                list.appendChild(li);
            });
        });
};

let typingTimeout;

document.getElementById("messageInput").addEventListener("input", function () {
    window.connection.invoke("SendTyping", document.getElementById("conversationId").value);
});

window.connection.on("ReceiveTyping", function (userName) {
    const indicator = document.getElementById("typingIndicator");
    indicator.textContent = `${userName} is typing...`;
    indicator.style.display = "block";
    clearTimeout(typingTimeout);
    typingTimeout = setTimeout(() => indicator.style.display = "none", 2000);
});

function setupChatFeatureEvents() {
    
    const menuBtn = document.getElementById("menuBtn");
    const chatMenu = document.getElementById("chatMenu");
    if (menuBtn && chatMenu) {
        menuBtn.onclick = function (e) {
            e.stopPropagation();
            chatMenu.style.display = chatMenu.style.display === "none" ? "block" : "none";
        };
        document.addEventListener("click", function hideMenu(e) {
            if (chatMenu.style.display === "block" && !chatMenu.contains(e.target) && e.target !== menuBtn) {
                chatMenu.style.display = "none";
            }
        }, { once: true });
    }

    // Emoji picker (simple prompt for demo)
    const emojiBtn = document.getElementById("emojiBtn");
    const emojiPicker = document.getElementById("emojiPicker");
    const messageInput = document.getElementById("messageInput");

    if (emojiBtn && emojiPicker && messageInput) {
        emojiBtn.onclick = function (e) {
            e.stopPropagation();
            emojiPicker.style.display = emojiPicker.style.display === "none" ? "block" : "none";
        };
        // Hide picker when clicking outside
        document.addEventListener("click", function hideEmojiPicker(e) {
            if (emojiPicker.style.display === "block" && !emojiPicker.contains(e.target) && e.target !== emojiBtn) {
                emojiPicker.style.display = "none";
            }
        }, { once: true });

        emojiPicker.addEventListener("emoji-click", function (event) {
            messageInput.value += event.detail.unicode;
            emojiPicker.style.display = "none";
            messageInput.focus();
        });
    }

    // File upload
    const fileInput = document.getElementById("fileInput");
    if (fileInput) {
        fileInput.onchange = function () {
            const file = this.files[0];
            if (!file) return;
            const formData = new FormData();
            formData.append("file", file);
            formData.append("conversationId", document.getElementById("conversationId").value);
            fetch("/Chat/UploadFile", { method: "POST", body: formData })
                .then(res => res.json())
                .then(data => {
                    window.connection.invoke("SendMessage", {
                        conversationId: document.getElementById("conversationId").value,
                        senderId: document.getElementById("senderId").value,
                        content: `<img src='${data.url}' class='img-fluid rounded' style='max-width:180px;' />`,
                        sentAt: new Date().toISOString(),
                        status: 0
                    });
                });
        };
    }
}

setupChatFeatureEvents();
