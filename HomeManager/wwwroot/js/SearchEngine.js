document.addEventListener("DOMContentLoaded", function () {
    const form = document.getElementById("searchForm");
    const input = document.getElementById("searchInput");
    const resultsDiv = document.getElementById("searchResults");

    form.addEventListener("submit", async function (e) {
        e.preventDefault(); // prevent actual form submission

        const query = input.value.trim();
        if (query.length < 2) {
            resultsDiv.innerHTML = "";
            return;
        }

        try {
            const res = await fetch(`/Search/GetResults?query=${encodeURIComponent(query)}`);
            if (!res.ok) throw new Error("Failed to fetch");

            const data = await res.json();
            resultsDiv.innerHTML = "";

            data.forEach(item => {
                const div = document.createElement("div");
                div.textContent = item.name || item.homeName; // adjust as needed
                resultsDiv.appendChild(div);
            });
        } catch (err) {
            console.error("Search failed:", err);
        }
    });
});