// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


// setInterval(function() {
//     $.get('/Home/GetLatestEstates', function(data) {
//         $('#estateCarousel .carousel-inner').html(data);
//     });
// }, 10000); 
console.log('site.js loaded');
window.addEventListener('DOMContentLoaded', function() {
    document.body.classList.add('loaded');
});
window.updateUnreadMessages = function() {
    console.log('updateUnreadMessages called');
    fetch('/Chat/GetUnreadCount')
        .then(res => res.json())
        .then(data => {
            console.log('Unread count from backend:', data.unreadCount);
            const badge = document.getElementById('unread-badge');
            const exclamation = document.getElementById('menu-exclamation');
            if (badge && exclamation) {
                if (data.unreadCount > 0) {
                    badge.textContent = data.unreadCount;
                    badge.style.display = 'inline-block';
                    exclamation.style.display = 'inline-block';
                } else {
                    badge.style.display = 'none';
                    exclamation.style.display = 'none';
                }
            } else {
                console.warn('Badge or exclamation not found in DOM');
            }
        })
        .catch(err => {
            console.error('Fetch error:', err);
        });
}

document.addEventListener('DOMContentLoaded', function () {
    const input = document.getElementById('UploadedImages');
    if (input) {
        input.addEventListener('change', function () {
            const preview = document.getElementById('imagePreview');
            preview.innerHTML = '';
            Array.from(input.files).forEach(file => {
                if (file.type.startsWith('image/')) {
                    const reader = new FileReader();
                    reader.onload = function (e) {
                        const img = document.createElement('img');
                        img.src = e.target.result;
                        img.className = 'me-2 mb-2';
                        img.style.height = '80px';
                        img.style.borderRadius = '8px';
                        preview.appendChild(img);
                    };
                    reader.readAsDataURL(file);
                }
            });
        });
    }
});

//document.addEventListener('DOMContentLoaded', function () {
//    document.querySelectorAll('.zoomable-image').forEach(function (img) {
//        img.addEventListener('click', function () {
//            img.classList.toggle('zoomed');
//        });
//    });
//});

document.addEventListener('DOMContentLoaded', function() {
    updateUnreadMessages();
});

const chatChannel = new BroadcastChannel('chat-sync');
chatChannel.onmessage = function(event) {
    if (event.data === 'refreshUnread') {
        window.updateUnreadMessages && window.updateUnreadMessages();
    }
};

window.updateUnreadMessages && window.updateUnreadMessages();
chatChannel.postMessage('refreshUnread');

//function attachChatListHandlers() {
//    document.querySelectorAll(".open-chat-link").forEach(function (link) {
//        link.addEventListener("click", async function () {
//            const conversationId = this.dataset.conversationId;
//            const parentLi = this.closest(".conversation-item");
//            if (parentLi) {
//                const badge = parentLi.querySelector(".badge.bg-danger");
//                if (badge) badge.remove();
//            }
//            fetch(`/Chat/ChatBoxPartial?conversationId=${conversationId}`)
//                .then(response => {
//                    if (!response.ok) throw new Error("Failed to load chat box.");
//                    return response.text();
//                })
//                .then(html => {
//                    document.getElementById("chatBoxContainer").innerHTML = "";
//                    document.getElementById("chatBoxContainer").innerHTML = html;
//                    setupChatFeatureEvents();
//                    if (window.prepareChatBox) window.prepareChatBox();
//                })
//                .catch(err => {
//                    alert("Could not load chat: " + err);
//                });
//        });
//    });
//}


