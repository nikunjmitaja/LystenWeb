
function SweetSaveSuccess(text) {
    swal({
        title: "Success!",
        text: text,
        type: "success",
        timer: 1000,
        showConfirmButton: false
    });
}


function SweetUpdateSuccess(status) {

    swal({
        title: "Success!",
        text: "Your row has been Updated.",
        type: "success",
        timer: 1000,
        showConfirmButton: false, animation: false
    });

    if (status !== 'new') {

        $('#myModal2 .close').click();
        $(".modal-backdrop").remove();

    }
}

function SweetCancelSuccess(text) {
    swal({
        title: "Cancelled!",
        text: text,
        type: "error",
        timer: 1000,
        showConfirmButton: false
    });
}