window.navigation.addEventListener('navigate', (event) => {
    const toUrl = new URL(event.destination.url);

    event.intercept({
        async handler() {
            try {
                $("#containerRender").load(toUrl.pathname + toUrl.search, function (responseTxt, statusTxt, xhr) {
                    if (statusTxt == "success") {
                        updateScript("/ViewFiles/" + toUrl.pathname.split("/")[toUrl.pathname.split("/").length - 1] + ".js");
                        const linkPartialView = $('#sheetPartialView')[0];      
                        linkPartialView.href = "/ViewFiles/" + toUrl.pathname.split("/")[toUrl.pathname.split("/").length - 1] + ".css";
                    }
                    if (statusTxt == "error") {
                        console.error("Error fetching or processing data:", xhr.statusText);
                    }
                });
            } catch (error) {
                console.error("Error fetching or processing data:", error);
            }
        }
    });
});

window.onbeforeunload = function () {
    const toUrl = new URL(ewindow.location.href);
    window.location.href = toUrl.origin + "/Razeon";
    return 'Are you sure you want to leave this page?';
};

function updateScript(scriptUrl) {
    var existingScript = $('#scriptPartialView')[0];
    if (existingScript) {
        existingScript.parentNode.removeChild(existingScript);
    }
    
    var newScript = document.createElement('script');
    newScript.id = 'scriptPartialView';
    newScript.src = scriptUrl;
    
    document.head.appendChild(newScript);

    $(".music-card-item").on("click", function () {
        var itemId = $(this).data("itemid");
        var itemType = $(this).data("itemtype");
        ClickedCardMusic(itemId, itemType);
    });
}