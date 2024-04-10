
function searchrecords(actionType, pageNumber, back, userid) {

    var formdata = [];
    if (actionType != "PatientRecords") {
        formdata = $('#recordsForm').serializeArray();
    }
    formdata.push({ name: "pageNumber", value: pageNumber });
    formdata.push({ name: "actionType", value: actionType });
    formdata.push({ name: "back", value: back });
    formdata.push({ name: "UserId", value : userid})
    var form = $.param(formdata);

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

function recordsoperation(dataObeject) {

    $.ajax({
        url: '/AdminRecords/DBOperations',
        type: 'POST',
        data: dataObeject,
        success: function (data) {
            $('#recordsTable').html(data);
            if (dataObeject.actionType == 'DeletePermanantly') {
                searchrecords('SearchRecords', '1');
            }
        },
        error: function () {
            console.error('Error loading partial view.');
        }
    });

}
