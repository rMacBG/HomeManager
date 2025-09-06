//document.addEventListener("DOMContentLoaded", function () {
//    const form = document.getElementById("searchForm");
//    const input = document.getElementById("searchInput");
//    const resultsDiv = document.getElementById("searchResults");

//    form.addEventListener("submit", async function (e) {
//        e.preventDefault();

//        const query = input.value.trim();
//        if (query.length < 2) {
//            resultsDiv.innerHTML = "";
//            return;
//        }
//        try {
//            const res = await fetch(`/Search/GetResults?query=${encodeURIComponent(query)}`);
//            if (!res.ok) {
//                throw new Error(`HTTP error: ${res.status}`);
//            }
//            const data = await res.json();
//            resultsDiv.innerHTML = "";

//            if (data.length === 0) {
//                resultsDiv.textContent = "No results found.";
//            } else {
//                data.forEach(item => {
//                    const div = document.createElement("div");

//                    div.textContent = `${item.name} (${item.type})`;
//                    resultsDiv.appendChild(div);
//                });
//            }
//        } catch (err) {
//            console.error("Search failed:", err);
//            resultsDiv.textContent = "An error occurred during search.";
//        }
//    });
//});
console.log("SearchEngine.js is loaded!");
document.addEventListener("DOMContentLoaded", function () {
    const form = document.getElementById("searchForm");
    const input = document.getElementById("searchInput");
    const resultsDiv = document.getElementById("searchResults");

   
    //form.addEventListener("submit", function(e) {
    //});

    //input.addEventListener("keydown", function(e) {
       
    //});

    input.addEventListener("keyup", async function (e) {
        const query = input.value.trim();
        resultsDiv.innerHTML = "";
        if (query.length < 2) {
            resultsDiv.style.display = "none";
            return;
        }
        resultsDiv.style.display = "block";
        try {
            const res = await fetch(`/Search/GetResults?query=${encodeURIComponent(query)}`);
            if (!res.ok) throw new Error(`HTTP error: ${res.status}`);
            const data = await res.json();
            if (data.length === 0) {
                resultsDiv.textContent = "No results found.";
            } else {
                data.slice(0, 6).forEach(item => {
                    const link = document.createElement("a");
                    link.textContent = `${item.name} (${item.type})`;
                    link.style.display = "block";
                    link.style.cursor = "pointer";
                    link.href = item.type === "Home"
                        ? `/Homes/Details/${item.id}`
                        : item.type === "User"
                            ? `/Users/Profile/${item.id}`
                            : "#";
                    resultsDiv.appendChild(link);
                });
                const viewAll = document.createElement("a");
                viewAll.href = `/Search/List?query=${encodeURIComponent(query)}`;
                viewAll.textContent = "View all results";
                viewAll.style.display = "block";
                viewAll.style.marginTop = "10px";
                viewAll.style.fontWeight = "bold";
                resultsDiv.appendChild(viewAll);
            }
        } catch (err) {
            console.error("Search failed:", err);
            resultsDiv.textContent = "An error occurred during search.";
        }
    });
});