function NotifyRequest(request, textMensage) {
    var color = ""
    if (request == 200) {
        color = "bg-success";
    }else{
        color = "bg-danger";
    }

    var Toast = Swal.mixin({
            toast: true,
            position: 'top-end',
            iconColor: 'white',
            customClass: {
                popup: color,
            },
            showConfirmButton: false,
            timer: 1500,
        });
    if (request == 200) {
        Toast.fire({
            icon: 'success',
            title: textMensage,
        });
    }
    else
    {
        Toast.fire({
            icon: 'error',
            title: textMensage,
        });
    }
}
