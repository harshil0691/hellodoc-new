

function adminDashload(dataObject) {

    $.ajax({
        url: '/' + dataObject.controller + '/' + dataObject.actionType,
        type: 'POST',
        data: { actionType: dataObject.action,userid: dataObject.UserId },
        success: function (data) {
            $('#mainDashContent').html(data);

            const dropdownParents = document.querySelectorAll('.nav-link');
            dropdownParents.forEach(parent => {
                parent.classList.remove('active');
            });
            var ele = document.getElementById(dataObject.id);
            ele.classList.add('active');


            if (dataObject.actionType == 'GetView') {
                searchrecords(dataObject.action, '1', dataObject.Back);
            }
           
        },
        error: function () {
            console.error('Error loading partial view.');
        }
    });
}

function ShowModal(DataObject) {
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
                $('#mainDashContent').html(data);
            }

            if (DataObject.ActionType == "dashboard") {
                loadPartialView(localStorage.getItem("StatusTab"),'1');
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
            $('#mainDashContent').html(data);
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
            $('#mainDashContent').html(data);
        },
        error: function () {
            console.error('Error loading partial view.');
        }
    });
}

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
                $('#mainDashContent').html(data);
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
            $('#mainDashContent').html(data);
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