
function searchrecords(actionType, pageNumber, back, userid) {
    event.preventDefault();
    var formdata = [];
    if (actionType != "PatientRecords") {
        formdata = $('#recordsForm').serializeArray();
    }
    formdata.push({ name: "pageNumber", value: pageNumber });
    formdata.push({ name: "actionType", value: actionType });
    formdata.push({ name: "back", value: back });
    formdata.push({ name: "UserId", value : userid})
    var form = $.param(formdata);

    if ($('#recordsForm').valid()) {
        $.ajax({
            url: '/AdminRecords/GetTable',
            type: 'POST',
            data: form,
            success: function (data) {
                if (actionType == "PatientRecords") {
                    $('#mainDashContent').html(data);
                }
                else {
                    $('#recordsTable').html(data);
                }
            },
            error: function () {
                console.error('Error loading partial view.');
            }
        });
    }
    
}

function GetRecordsView(dataObject) {

    $.ajax({
        url:'/AdminRecords/GetView',
        type: 'POST',
        data: { actionType: dataObject.action, userid: dataObject.UserId },
        success: function (data) {
            const dropdownParents = document.querySelectorAll('.nav-link');
            dropdownParents.forEach(parent => {
                parent.classList.remove('active');
            });
            localStorage.setItem("DashTab", 'records');
            var element = document.querySelectorAll('[href="#records"]');
            element.forEach(a => {
                a.classList.add('active');
            });
            console.log('dasd');
            $('#mainDashContent').html(data);

            searchrecords(dataObject.action, '1', dataObject.Back);
        },
        error: function () {
            console.error('Error loading partial view.');
        }
    });
}


function recordsoperation(dataObeject) {
    $.ajax({
        url: '/AdminRecords/DBOperations',
        type: 'POST',
        data: dataObeject,
        success: function (data) {
            if (dataObeject.actionType == 'DeletePermanantly' ) {
                searchrecords('SearchRecords', '1');
            } else if (dataObeject.actionType == 'Unblock') {
                searchrecords('BlockedHistory', '1');
            }
        },
        error: function () {
            console.error('Error loading partial view.');
        }
    });

}
