﻿$("#vendorsearch").on("keyup change", function () {
    partners({ actionType: 'partnerstable', pageNumber: '1' });
});

$("#proffesiontype").on("change", function () {
    partners({ actionType: 'partnerstable', pageNumber: '1' });
});

partners({ actionType: 'partnerstable', pageNumber: '1' });

function partners(dataObject) {

    if (dataObject.actionType == "partnerstable") {
        dataObject.professionalType = $("#proffesiontype").val();
        dataObject.search = $("#vendorsearch").val();
    }

    if (dataObject.back == true) {
        partners({ actionType: 'Partners' });
    }

    $.ajax({
        url: '/AdminPartners/GetView',
        type: 'POST',
        data: dataObject,
        success: function (data) {
            if (dataObject.actionType == "partnerstable") {
                $('#partnerTable').html(data);
            } else {
                $('#mainDashContent').html(data);
            }
        },
        error: function () {
            console.error('Error loading partial view.');
        }
    });
}


function bussinessActions(actionType, vendorid) {
    event.preventDefault();
    var formdata = $('#business').serializeArray();
    formdata.push({ name: "Vendorid", value: vendorid });
    formdata.push({ name: "actionType", value: actionType });
    var form = $.param(formdata);

    if ($('#business').valid()) {
        $.ajax({
            url: '/AdminPartners/DBOperations',
            type: 'POST',
            data: form,
            success: function (data) {
                if (actionType == 'new_business' || actionType == 'delete_business') {
                    partners({ actionType: 'Partners' });
                    toastr.success("Vendor Created Successfully");
                } else {
                    partners({ actionType: 'edit_business', venorid: vendorid });
                    toastr.success("Vendor Edited Successfully");
                }
            },
            error: function () {
                console.error('Error loading partial view.');
            }
        });
    }

}

function deleteBusiness(actionType,vendorid) {

    $.ajax({
        url: '/AdminPartners/DeleteBusiness',
        type: 'POST',
        data: { Vendorid: vendorid },
        success: function (data) {
            partners({ actionType: 'Partners' });
            toastr.success("Vendor Deletes Successfully");
        },
        error: function () {
            console.error('Error loading partial view.');
        }
    });

}

function businessEdit(actionType, vendorid) {
    if (actionType == 'edit') {
        var inputs = document.getElementsByClassName('form-control');
        for (var i = 0; i < inputs.length; i++) {
            inputs[i].disabled = false;
        }

        document.getElementById('businessedit').style.display = 'none';
        document.getElementById('businesssave').style.display = 'block';
    }

}