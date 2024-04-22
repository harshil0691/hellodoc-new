
function GetAccessView(dataObject) {
    dataObject.datestring = date.toISOString();
    $.ajax({
        url: '/AdminAccess/GetAccessView',
        type: 'POST',
        data: dataObject,
        success: function (data) {
            var element = document.querySelectorAll('[href="#' + localStorage.getItem("DashTab") + '"]');
            element.forEach(a => {
                a.classList.remove('active');
            });
            localStorage.setItem("DashTab", 'access');
            var element = document.querySelectorAll('[href="#access"]');
            element.forEach(a => {
                a.classList.add('active');
            });

            $('#mainDashContent').html(data);
            if (dataObject.actionType == 'scheduling') {
                ShiftCalender('month');
            }
        },
        error: function () {
            console.error('Error loading partial view.');
        }
    });
}

function CRUDAccess(actionType) {

    var regionlist = [];
    var checkboxes = document.querySelectorAll(".regioncheck");

    checkboxes.forEach(function (checkbox) {
        if (checkbox.checked) {
            regionlist.push(checkbox.value);
        }
    });


    var formdata = [];
    formdata = $('#adminProfile').serializeArray();
    formdata.push({ name: "actionType", value: actionType });
    formdata.push({ name: "selectedRegion", value: regionlist });
    var form = $.param(formdata);

    $.ajax({
        url: '/AdminAccess/CRUDAccess',
        type: 'POST',
        data: form,
        success: function (data) {
            if (data.type != 'error') {
                toastr.success(data.message);
            }
            GetAccessView({ actionType : "accountAccess" });
        },
        error: function () {
            console.error('Error loading partial view.');
        }
    });
}