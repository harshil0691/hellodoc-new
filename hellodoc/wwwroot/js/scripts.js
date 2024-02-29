

function darkmode() {

    var element = document.body;
    console.log(element);

    if (element.dataset.bsTheme === "light") {
        element.dataset.bsTheme = "dark";
        document.getElementById("dashboard").style.backgroundColor = 'black';
        document.getElementById("bgcolor").style.backgroundColor = 'black';
        document.getElementById("newtable").style.backgroundColor = 'black';
        document.getElementById("main").style.backgroundColor = 'black';

    } else {
        element.dataset.bsTheme = "light";
        document.getElementById("dashboard").style.backgroundColor = 'white';
        document.getElementById("bgcolor").style.backgroundColor = 'white';
        document.getElementById("main").style.backgroundColor = 'white';
    }
    console.log(element.dataset.bsTheme);
}

function edit() {

    document.getElementById("b1").style.display = 'block';
    document.getElementById("b2").style.display = 'block';
    document.getElementById("e1").style.display = 'none';

    var inputs = document.getElementsByClassName("form-control");

    for (var i = 0; i < inputs.length; i++) {
        inputs[i].disabled = false;
    }

}


function cancel() {
    location.reload()
}


var requestValue;

function me1(button) {
    requestValue = button.value;
}
function se1(button) {
    requestValue = button.value;
}

function create_request() {
    console.log(requestValue);
    if (requestValue == "me") {
        window.location.href = '@Url.Action("request_me","Patient")';
    }
    else {
        window.location.href = '@Url.Action("request_someone","Patient")'
    }

}

const phoneInputField = document.querySelector("#phone");
const phoneInput = window.intlTelInput(phoneInputField, {
    utilsScript:
        "https://cdnjs.cloudflare.com/ajax/libs/intl-tel-input/17.0.8/js/utils.js",
});


$(document).ready(function () {
    $("#customSearch").on("keyup", function () {
        var value = $(this).val().toLowerCase();
        $("#newtable tr").filter(function () {
            $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
        });
    });
});


function ShowModal(Requestid, PatientName, Modalname) {
    console.log(1);
    $.ajax({
        url: '/AdminDash/Openmodal',
        type: 'Post',
        data: { Requestid: Requestid, Patientname: PatientName, Modalname: Modalname },
        success: function (data) {

            $('#partialContainer1').html(data);
            $('#myModal').modal('show');

        },
        error: function (e) {
            console.log(e);
        }
    });
}