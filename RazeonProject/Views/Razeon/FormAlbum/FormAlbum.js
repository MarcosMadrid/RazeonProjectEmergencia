var fileInput = document.getElementById('imgInputAlbum');
var previewImage = document.getElementById('previewImage');
var albumId = "";

fileInput.addEventListener('change', function (event) {
    const file = event.target.files[0];
    if (file) {
        const reader = new FileReader();
        reader.onload = function (e) {
            previewImage.src = e.target.result;
            previewImage.style.display = 'block';
        };
        reader.readAsDataURL(file);
    } else {
        previewImage.src = '#';
        previewImage.style.display = 'none';
    }
});

function ChangeApperance(album) {
    var button = $("#buttonActionType");
    button.removeAttr("onclick");
    button.on("click", function () {
        EditAlbum(album);
    });
    button.text("Editar Album");

    var blob = new Blob([album.Image], { type: "image/*" }); 
    var file = new File([blob], "album_image.jpg");
    $("#imgInputAlbum")[0].files[0] = file;
    $("#nameInputAlbum")[0].value = album.name;
}

function AddRow(item) {
    var rowHead = $('#table-data thead th');
    var newRow = $('<tr class="row-item-list" ></tr>');
    for (var i = 0; i < item.length; i++) {
        for (var z = 0; z < rowHead.length; z++) {
            var columnHeader = $(rowHead[z]).text().trim();
            if(columnHeader === item[i].Name) {
                if (item[i].Type == "Byte[]") {
                    newRow.append('<td><input type="file" class="form-control" name="' + item[i].Name + '"></td>');
                } else {
                    newRow.append('<td><textarea class="form-control border-0 bg-transparent p-0 text-center" name="' + item[i].Name + '" rows="auto" type="text"></textarea></td>');
                }
            }
        }
    } 

    var buttons = "<td class='w-auto align-middle p-0 buttons-table-container'>" +
        "<button class='col-auto mr-auto btn btn-outline-primary py-1 px-2 mx-2 border-0' type='button' onclick='AddItem(this)'>" +
        "<i class='bi bi-save'></i>" +
        "</button>" +
        "<button class='col-auto mr-auto btn btn-outline-danger py-1 px-2 mx-2 border-0' type='button' onclick='DeleteItem(this)'>" +
        "<i class='bi bi-trash'></i>" +
        "</button>" +
        "<button class='col-auto mr-auto btn btn-outline-success py-1 px-2 mx-2 border-0' type='button' onclick='GetItem(this)'>" +
        "<i class='bi bi-eye'></i>" +
        "</button>" +
        "</td>";
    newRow.append(buttons)
    $('#table-data tbody').append(newRow);
}

function GetItem(button) {
    var item = LoadItem(button);
    console.log(item);
}

function DeleteItem(button) {
    var item = LoadItem(button);
    DeleteCancion(item);
    $(button).closest("tr").remove();
    console.log(item);
}

function AddItem(button) {
    var item = LoadItem(button);
    PostCancion(item).then(function (response) {
        UpdateRow(response, button);
    });
}

function LoadItem(button) {
    var row = $(button).closest('.row-item-list')[0];
    var inputs = row.querySelectorAll('input, textarea');
    var item = {};

    for (const input of inputs) {
        if (input.type === "file") {
            item[input.name] = input.files[0];
        } else {
            item[input.name] = input.value;
        }
    }
    return item;
}

function CreateAlbum(idUser) {
    var imagAlbum = $("#imgInputAlbum")[0].files[0];
    var nombreAlbum = $("#nameInputAlbum")[0].value;
    var artista = idUser;

    var formData = new FormData();
    formData.append("Image", imagAlbum);
    formData.append("ArtistId", artista);
    formData.append("Name", nombreAlbum);
    formData.append("Released", null); 
    formData.append("Id", null); 

    $.ajax({
        url: '/Razeon/CreateAlbum',
        type: 'POST',
        data: formData,
        processData: false,
        contentType: false,
        success: function (response) {
            console.log("Creado" + response);
            ChangeApperance(response);
            NotifyRequest(200, "Album guardado");
            albumId = response.id;
        },
        error: function (xhr, status, error) {
            console.log("eRROR" + error + " | " + status);
            NotifyRequest(status, error + " | " + status);
        }
    })
}

function EditAlbum(album) {
    var imagAlbum = $("#imgInputAlbum")[0].files[0];
    var nombreAlbum = $("#nameInputAlbum")[0].value;
    var artista = album.artistId;

    var formData = new FormData();
    formData.append("Image", imagAlbum);
    formData.append("ArtistId", artista);
    formData.append("Name", nombreAlbum);
    formData.append("Released", null);
    formData.append("Id", album.id)

    $.ajax({
        url: '/Razeon/EditAlbum',
        type: 'PUT',
        data: formData,
        processData: false,
        contentType: false,
        success: function (response) {
            console.log("Editado" + response);
            ChangeApperance(response);
            NotifyRequest(200, "Album editado");
        },
        error: function (xhr, status, error) {
            console.log("eRROR" + error + " | " + status);
            NotifyRequest(status, error + " | " + status);
        }
    })
}

function PostCancion(item) {
    return new Promise(function (resolve, reject) {
        var formData = new FormData();
        formData.append("AlbumId", albumId)
        formData.append("Image", item.Image);
        formData.append("Title", item.Title);
        formData.append("FileAudio", item.FileAudio);

        $.ajax({
            url: '/Razeon/CreateTrack',
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            success: function (response) {
                console.log("Creado" + response);
                NotifyRequest(200, "Canción guardada");
                resolve(response);
            },
            error: function (xhr, status, error) {
                console.log("eRROR" + error + " | " + status);
                NotifyRequest(status, error + " | " + status);
                reject(error);
            }
        });
    });
}

function DeleteCancion(item) {
    var formData = new FormData();
    formData.append("idTrack", item.id)

    $.ajax({
        url: '/Razeon/DeleteTrack',
        type: 'DELETE',
        data: formData,
        processData: false,
        contentType: false,
        success: function (response) {
            console.log("Elimainado" + response);
            NotifyRequest(200, "Canción eliminada");
            albumId = response.id;
        },
        error: function (xhr, status, error) {
            console.log("eRROR" + error + " | " + status);
            NotifyRequest(status, error + " | " + status);
        }
    });
}

function GetCancionesAlbum(idAlbum) {
    var formData = new FormData();
    formData.append("idAlbum", idAlbum)

    $.ajax({
        url: '/Razeon/GetAlbumTracks',
        type: 'GET',
        data: formData,
        processData: false,
        contentType: false,
        success: function (response) {
            console.log("Get Canciones");
        },
        error: function (xhr, status, error) {
            console.log("eRROR" + error + " | " + status);
        }
    });
}

function UpdateRow(item, button) {
   var row = $(button).closest("tr");
    var inputs = row.find("textarea, input");
    inputs.each(function () {
        var name = $(this).attr("name"); 
        if (name && item.hasOwnProperty(name.toLowerCase())) {
            $(this).val(item[name.toLowerCase()]);
        }
    });

}