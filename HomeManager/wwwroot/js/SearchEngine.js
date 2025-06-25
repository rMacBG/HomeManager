document.addEventListener("DOMContentLoaded", function () {
    const form = document.getElementById("searchForm");
    const input = document.getElementById("searchInput");
    const resultsDiv = document.getElementById("searchResults");

    form.addEventListener("submit", async function (e) {
        e.preventDefault();

        const query = input.value.trim();
        if (query.length < 2) {
            resultsDiv.innerHTML = "";
            return;
        }
        try {
            const res = await fetch(`/Search/GetResults?query=${encodeURIComponent(query)}`);
            if (!res.ok) {
                throw new Error(`HTTP error: ${res.status}`);
            }
            const data = await res.json();
            resultsDiv.innerHTML = "";

            if (data.length === 0) {
                resultsDiv.textContent = "No results found.";
            } else {
                data.forEach(item => {
                    const div = document.createElement("div");
                    
                    div.textContent = `${item.name} (${item.type})`;
                    resultsDiv.appendChild(div);
                });
            }
        } catch (err) {
            console.error("Search failed:", err);
            resultsDiv.textContent = "An error occurred during search.";
        }
    });
});