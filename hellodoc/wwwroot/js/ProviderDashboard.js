function adminDashload(dataObject) {

    $.ajax({
        url: '/' + dataObject.controller + '/' + dataObject.actionType,
        type: 'POST',
        data: dataObject,
        success: function (data) {
            if (dataObject.actionType == "AcceptRequest") {
                if (data == "ok") {
                    toastr.success("Request Is Accepted");
                } else {
                    toastr.warning("Internanl Error");
                }
                loadPartialDashView('dashboard');
            } else {
                $('#mainDashContent').html(data);
            }

            const dropdownParents = document.querySelectorAll('.nav-link');
            dropdownParents.forEach(parent => {
                parent.classList.remove('active');
            });
            var ele = document.getElementById(dataObject.id);
            ele.classList.add('active');

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

var calltype = "";
var othercalltype = ""
function enccounterradio(clickType) {
    if (clickType == "housecall") {
        calltype = "HouseCall";
        othercalltype = "consult";

    } else {
        calltype = "Consult";
        othercalltype = "housecall";
    }
    var h = document.getElementById(clickType);
    h.classList.remove("btn-outline-info");
    h.classList.add("btn-info", "text-white");

    var c = document.getElementById(othercalltype);
    c.classList.remove("btn-info", "text-white");
    c.classList.add("btn-outline-info");
}


function loadActionView(DataObject) {

    $.ajax({
        url: '/DashActionView/LoadActionViews',
        type: 'POST',
        data: DataObject,
        success: function (data) {
            if (data.isfinalized) {
                ShowModal({ ActionType: 'finalizedencounter' })
            }
            else if (DataObject.ActionType == "Encounter") {
                $('#myModal').modal('hide');
                toastr.success(data.data);
            }
            else {
                $('#mainDashContent').html(data);
            }

            if (DataObject.ActionType == "dashboard") {
                loadPartialView(localStorage.getItem("StatusTab"), '1');
            }

        },
        error: function () {
            console.error('Error loading partial view.');
        }
    });
}

function loadActionView1(DataObject) {

    if (DataObject.ActionType == "Encounter") {
        DataObject.callType = calltype;
    }

    console.log(DataObject);

    $.ajax({
        url: '/DashActionView/' + DataObject.ActionType,
        type: 'POST',
        data: DataObject,
        success: function (data) {
            if (data.isfinalized) {
                ShowModal({ ActionType: 'finalizedencounter' })
            }
            else if (DataObject.ActionType == "Encounter" || DataObject.ActionType == "TransferToAdmin") {
                $('#myModal').modal('hide');
                toastr.success(data);
                //if (DataObject.ActionType == "TransferToAdmin"){
                //    loadPartialView('pending', '1');
                //}else{
                //    loadPartialView('active', '1');
                //}
                window.location.reload();
            }
            else {
                $('#mainDashContent').html(data);
            }

            if (DataObject.ActionType == "dashboard") {
                loadPartialView(localStorage.getItem("StatusTab"), '1');
            }

        },
        error: function () {
            console.error('Error loading partial view.');
        }
    });
}

function FormSubmitAction(actionType, id, tab) {

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