

function darkmode() {
    var elements = document.querySelectorAll('[data-bs-theme]');

    // Check if dark mode is enabled
    var isDarkMode = localStorage.getItem('darkMode') === 'true';

    elements.forEach(function (element) {
        console.log(element.dataset.bsTheme);
        if (isDarkMode) {
            element.dataset.bsTheme = "light";
        } else {
            element.dataset.bsTheme = "dark";
        }
    });

    var elements1 = document.querySelectorAll('.bgcolor');
    elements1.forEach(function (element1) {
        element1.classList.toggle('dark-mode');
    });

    var elements2 = document.querySelectorAll('.bgcolor1');
    elements2.forEach(function (element2) {
        element2.classList.toggle('dark-mode');
    });

    // Toggle dark mode setting
    localStorage.setItem('darkMode', !isDarkMode);
}

document.addEventListener('DOMContentLoaded', function () {
    var element = document.body;
    var isDarkMode = localStorage.getItem('darkMode') === 'true';
    console.log(isDarkMode);

    if (isDarkMode) {
        element.dataset.bsTheme = "dark";
        var elements1 = document.querySelectorAll('.bgcolor');
        elements1.forEach(function (element1) {
            element1.classList.toggle('dark-mode');
        });

        var elements2 = document.querySelectorAll('.bgcolor1');
        elements2.forEach(function (element2) {
            element2.classList.toggle('dark-mode');
        });
    }
});

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


// patient request  

async function emailExists() {
    const email = document.getElementById("floatingInput12").value;

    const response = await fetch('/Patient/patient_request/checkemail/' + email);

    const data = await response.json();

    if (data.exists) {

        document.getElementById("Password1").style.display = 'none';
        document.getElementById("Password2").style.display = 'none';
    } else {
        document.getElementById("Password1").style.display = 'block';
        document.getElementById("Password2").style.display = 'block';
    }
}

function samepass() {
    pass1 = document.getElementById("p1").value;
    pass2 = document.getElementById("cp1").value;

    if (pass1 !== pass2) {
        document.getElementById("cnp1").innerHTML = "password and confirm password not  same";
    }

}

const phoneInputField = document.querySelector("#phone");
const phoneInput = window.intlTelInput(phoneInputField, {
    utilsScript:
        "https://cdnjs.cloudflare.com/ajax/libs/intl-tel-input/17.0.8/js/utils.js",
});

Swal.fire({
    title: "Information",
    text: "When submitting a request you must provide the correct information for the patient or the responsibly party.Failure to provide correct e-mail and phone number will delay service or be declined ",
    icon: "warning",
    showCancelButton: false,
    confirmButtonColor: "#3085d6",
    cancelButtonColor: "#d33",
    confirmButtonText: "Ok"
})
