function fetchArtistPage(pageNumber) {
    $('#renderArtistPage').load('/Razeon/GetArtistPage', { page: pageNumber }, function (response, status, xhr) {
        if (status == "error") {
            console.error("Error fetching artist page:", xhr.status, xhr.statusText);
        }
    });
}

$(document).ready(function () {
    fetchArtistPage(1);
});

$('#pagination').on('click', '.page-link', function (e) {
    e.preventDefault();
    var pageNumber = $(this).data('page');
    fetchArtistPage(pageNumber);
});
