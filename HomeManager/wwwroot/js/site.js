// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

setInterval(function() {
    $.get('/Home/GetLatestEstates', function(data) {
        $('#estateCarousel .carousel-inner').html(data);
    });
}, 10000); 

window.addEventListener('DOMContentLoaded', function() {
    document.body.classList.add('loaded');
});

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

document.addEventListener('DOMContentLoaded', function () {
    document.querySelectorAll('.zoomable-image').forEach(function (img) {
        img.addEventListener('click', function () {
            img.classList.toggle('zoomed');
        });
    });
});
