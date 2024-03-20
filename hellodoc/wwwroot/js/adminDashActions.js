

function adminDashload(dataObject) {
    console.log(dataObject);
    $.ajax({
        url: '/AdminDash/' + dataObject.actionType,
        type: 'POST',
        success: function (data) {
            $(dataObject.tab).html(data);

            const dropdownParents = document.querySelectorAll('.nav-link');
            dropdownParents.forEach(parent => {
                parent.classList.remove('active');
            });
            var ele = document.getElementById('provideractive');
            ele.classList.add('active');

            const dropdownParents1 = document.querySelectorAll('.tab-pane');
            dropdownParents1.forEach(parent => {
                parent.classList.remove('active');
            });

            var ele = document.getElementById('provider');
            ele.classList.add('active');
        },
        error: function () {
            console.error('Error loading partial view.');
        }
    });
}



function ShowModal(DataObject) {
    console.log(DataObject);
    $.ajax({
        url: '/DashActionView/Openmodal',
        type: 'Post',
        data: DataObject,
        success: function (data) {
            console.log(1)
            $('#partialContainer').html(data);
            $('#myModal').modal('show');
        },
        error: function (e) {
            console.log(e);
        }
    });
}


function loadActionView(DataObject) {

    $.ajax({
        url: '/DashActionView/LoadActionViews',
        type: 'POST',
        data: DataObject,
        success: function (data) {
            if (data.isfinalized) {
                ShowModal({ ActionType: 'finalizedencounter'})
            } else {
                $('#dashboard').html(data);
            }

            if (DataObject.ActionType == "dashboard") {
                switch (DataObject.activeid) {
                   
                    case 1 :
                        loadPartialView("new");
                        break;
                    case 2:
                        loadPartialView("pending");
                        break;
                    case 3:
                        loadPartialView("active");
                        break;
                    case 4:
                        loadPartialView("conclude");
                        break;
                    case 5:
                        loadPartialView("toclose");
                        break;
                    case 6:
                        loadPartialView("unpaid");
                    default:
                        loadPartialView("new");
                }
            }
            
        },
        error: function () {
            console.error('Error loading partial view.');
        }
    });
}

function FormSubmitAction(actionType,id,tab) {

    var formdata = $(id).serialize();

    $.ajax({
        url: '/DashActionView/' + actionType,
        type: 'POST',
        data: formdata,
        success: function (data) {
            $(tab).html(data);
        },
        error: function () {
            console.error('Error loading partial view.');
        }
    });
}

function FormSubmitAction1(actionType, id) {

    var formdata = $(id).serialize();

    $.ajax({
        url: '/AdminDash/' + actionType,
        type: 'POST',
        data: formdata,
        success: function (data) {
            $('#dashboard').html(data);
        },
        error: function () {
            console.error('Error loading partial view.');
        }
    });
}



//document.getElementById("check-header").addEventListener("click", function () {
//    var checkboxes = document.querySelectorAll(".custom-check");
//    checkboxes.forEach(function (checkbox) {
//        checkbox.checked = document.getElementById("check-header").checked;
//    });
//});

function deleteAll(requestid, actionType) {
    var selectedCheck = [];

    var checkboxes = document.querySelectorAll(".custom-check");

    checkboxes.forEach(function (checkbox) {
        if (checkbox.checked) {
            selectedCheck.push(checkbox.value);
        }
    });

    $.ajax({
        url: '/DashActionView/DocumentActions',
        type: 'POST',
        data: { selectedCheck: selectedCheck, requestid: requestid, actionType: actionType },
        success: function (data) {
            if (actionType != 'DownloadAll') {
                $('#dashboard').html(data);
            } else {
                var blob = new Blob([data], { type: "application/zip" });
                saveAs(blob, "downloadedDocuments.zip");
            }

        },
        error: function () {
            console.error('Error loading partial view.');
        }
    });

    console.log(selectedCheck);

}

function savedoc() {

    var formdata = new FormData();
    var fileupload = document.getElementById('file1');
    var f = fileupload.files[0];
    formdata.append('file1', f);

    console.log(formdata);

    $.ajax({
        url: '/DashActionView/view_uploads',
        type: 'POST',
        data: formdata,
        processData: false,
        contentType: false,
        success: function (data) {
            $('#dashboard').html(data);
        },
        error: function () {
            console.error('Error loading partial view.');
        }
    });
}


function OpenSwal(Requestid) {

    console.log(Requestid);

    const swalWithBootstrapButtons = Swal.mixin({
        customClass: {
            confirmButton: "btn btn-info m-2",
            cancelButton: "btn btn-outline-info m-2"
        },
        buttonsStyling: false
    });

    swalWithBootstrapButtons.fire({
        title: "Confirmation For Clear Case",
        text: "Are sure you want to clear this request? Once clear this request then you not able to see this request.",
        icon: "warning",
        showCancelButton: true,
        confirmButtonText: "Clear",
        cancelButtonText: "Cancel",
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: '/AdminDash/ClearCase',
                type: 'Post',
                data: { Requestid: Requestid },
                success: function (data) {

                },
                error: function (e) {
                    console.log(e);
                }
            });
        }
    });
}