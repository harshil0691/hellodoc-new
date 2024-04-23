

function saveStopNotification() {

    var idlist = [];
    var checkboxes11 = document.querySelectorAll(".providerscheck");

    checkboxes11.forEach(function (checkbox) {
        if (checkbox.checked) {
            idlist.push(checkbox.value);
        }
    });

    var totallist = [];
    var checkboxes11 = document.querySelectorAll(".providerscheck");

    checkboxes11.forEach(function (checkbox) {
        totallist.push(checkbox.value);
    });

    $.ajax({
        url: '/AdminProviders/stopnotification',
        type: 'POST',
        data: { idlist: idlist, totallist: totallist },
        success: function (data) {
            document.getElementById("ProvidersSave").style.display = "none";
            selectedCheck = idlist;
            toastr.success("Notification Changes Saved Successfully");
        },
        error: function () {
            console.error('Error loading partial view.');
        }
    });
}

function GetProvidersView(dataObject) {
    dataObject.datestring = date.toISOString();
    $.ajax({
        url: '/AdminProviders/GetProvidersView',
        type: 'POST',
        data: dataObject,
        success: function (data) {
            var element = document.querySelectorAll('[href="#' + localStorage.getItem("DashTab") + '"]');
            element.forEach(a => {
                a.classList.remove('active');
            });
            localStorage.setItem("DashTab", 'provider');
            var element = document.querySelectorAll('[href="#provider"]');
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

function loadProviderView(method, physicianid) {
    $.ajax({
        url: '/AdminProviders/' + method,
        type: 'POST',
        data: { physicianid: physicianid },
        success: function (data) {
            $('#mainDashContent').html(data);
        },
        error: function () {
            console.error('Error loading partial view.');
        }
    });
}

var physiciandid = 0;
var date = new Date();
var diff = 7 - date.getDay();
if (diff === 0) {
    diff = 7;
}
var sunday = new Date(date.getTime() + (diff * 24 * 60 * 60 * 1000));
var saturday = new Date();
saturday.setDate(sunday.getDate() + 6);

var schedulingtype;

function showcalender() {
    document.getElementById("month-input").showPicker();
}

function ShiftCalender(shifttype,regionid) {
    schedulingtype = shifttype;
    var datehere;
    var calendar = document.getElementById("cal");
    calendar.innerHTML = '';


    switch (schedulingtype) {
        case 'month':
            datehere = date.toLocaleDateString('en-US', { month: 'long' }) + ',' + date.getFullYear();
            calendar.innerHTML = '<input type="month" id="month-input" onchange="monthchange()"/>';
            break;
        case 'day':
            datehere = date.toLocaleDateString('en-US', { weekday: 'long' }) + ', ' + (date.toLocaleDateString('en-US', { month: 'long' })).substring(0, 3) + ' ' + date.getDate() + ', ' + date.getFullYear();
            calendar.innerHTML = '<input type="date" id="month-input" onchange="monthchange()"/>';
            break;
        case 'week':
            datehere = (sunday.toLocaleDateString('en-US', { month: 'long' })).substring(0, 3) + ' ' + sunday.getDate() + ' - ' + (saturday.toLocaleDateString('en-US', { month: 'long' })).substring(0, 3) + ' ' + saturday.getDate() + ' ' + saturday.getFullYear();
            calendar.innerHTML = '<input type="week" id="month-input" onchange="monthchange()"/>';

    }
    if (localStorage.getItem("loginAccount") != "Provider") {
        document.getElementById('day').classList.remove('btn-info', 'text-white');
        document.getElementById('week').classList.remove('btn-info', 'text-white');
        document.getElementById('month').classList.remove('btn-info', 'text-white');
        document.getElementById(schedulingtype).classList.add('btn-info', 'text-white');
    }

    $.ajax({
        url: '/AdminProviders/loadshift',
        type: 'POST',
        data: { datestring: date.toISOString(), sundaystring: sunday.toISOString(), saturdaystring: saturday.toISOString(), shifttype: shifttype,regionid:regionid },
        success: function (data) {
            
            $('#shiftTable').html(data);
            $('#datediv').text(datehere);
        },
        error: function () {
            console.error('Error loading partial view.');
        }
    });
}


function monthchange() {

    switch (schedulingtype) {
        case 'month':
            var selectmonth = document.getElementById("month-input").value;
            var year = parseInt(selectmonth.split("-")[0]);
            var month = parseInt(selectmonth.split("-")[1]);
            date.setMonth(month - 1);
            date.setFullYear(year);

            ShiftCalender('month');
            break;

        case 'day':
            var selectday = document.getElementById("month-input").value;
            var year = parseInt(selectday.split("-")[0]);
            var month = parseInt(selectday.split("-")[1]);
            var day = parseInt(selectday.split("-")[2]);
            date.setMonth(month - 1);
            date.setFullYear(year);
            date.setDate(day);

            ShiftCalender('day');
            break;

        case 'week':
            var selectweek = document.getElementById("month-input").value;
            // var year12 = parseInt(selectmonth.split("-")[0]);
            console.log(selectweek);
            // console.log(year12);
            //sunday.setDate(sunday.getDate() + 7);
            //saturday.setDate(saturday.getDate() + 7);
            //ShiftCalender('week');
            break;
    }

}

function nextCalendar() {
    switch (schedulingtype) {
        case 'month':
            date.setMonth(date.getMonth() + 1);
            ShiftCalender('month');
            break;

        case 'day':
            date.setDate(date.getDate() + 1);
            ShiftCalender('day');
            break;

        case 'week':
            sunday.setDate(sunday.getDate() + 7);
            saturday.setDate(saturday.getDate() + 7);
            ShiftCalender('week');
            break;
    }
}

function previousCalendar() {

    switch (schedulingtype) {
        case 'month':
            date.setMonth(date.getMonth() - 1);
            ShiftCalender('month');
            break;

        case 'day':
            date.setDate(date.getDate() - 1);
            ShiftCalender('day');
            break;
        case 'week':
            sunday.setDate(sunday.getDate() - 7);
            saturday.setDate(saturday.getDate() - 7);
            ShiftCalender('week');
            break;
    }

}

function providersModal(DataObject) {
    physiciandid = DataObject.physiciandid;
    $.ajax({
        url: '/AdminProviders/Openmodal',
        type: 'POST',
        data: DataObject,
        success: function (data) {
            $('#myModal').modal('hide');
            $('#partialContainer').html(data);
            $('#myModal').modal('show');
        },
        error: function (e) {
            console.log(e);
        }
    });
}

function shiftactions(actionType, formid) {
    event.preventDefault();
    $('#myModal').modal('hide');
    var daylist = [];
    var inputs = document.getElementsByClassName('daycheck');
    for (var i = 0; i < inputs.length; i++) {
        if (inputs[i].checked == true) {
            daylist.push(parseInt(inputs[i].value));
        }
    }
    var formdata = $(formid).serializeArray();
    formdata.push({ name: "Weekdays", value: JSON.stringify(daylist) });
    var form = $.param(formdata);

    if ($(formid).valid()) {
        $.ajax({
            url: '/AdminProviders/' + actionType,
            type: 'POST',
            data: form,
            success: function (data) {
                ShiftCalender(schedulingtype);
            },
            error: function () {
                console.error('Error loading partial view.');
            }
        });
    }

}

function rdShift(actionType) {
    
    $.ajax({
        url: '/AdminProviders/' + actionType,
        type: 'POST',
        success: function (data) {
            $('#myModal').modal('hide');
            ShiftCalender(schedulingtype);
        },
        error: function () {
            console.error('Error loading partial view.');
        }
    });

}

function checkShiftAvailability1() {

    var physician = $('#Physicianselect').val();
    var starttime = $('#stime').val();
    var endtime = $('#etime').val();
    var shiftdate = $('#dateInput').val();
    var submit = document.getElementById('shiftsave');



    $.ajax({
        url: '/AdminProviders/checkShiftAvailability',
        type: 'POST',
        data: { physicianid: physiciandid, starttime: starttime, endtime: endtime, shiftdate: shiftdate },
        success: function (data) {
            return data;
            //if (data == true) {
            //    submit.disabled = true;
            //    $('#shiftresult').text('you already have a shift in this interval');
            //} else {
            //    submit.disabled = false;
            //    $('#shiftresult').text('');
            //}
        },
        error: function (e) {
            console.log(e);
        }
    })

}

function CreateProvider() {
    var regionlist = [];
    var checkboxes = document.querySelectorAll(".regioncheck");

    checkboxes.forEach(function (checkbox) {
        if (checkbox.checked) {
            regionlist.push(checkbox.value);
        }
    });

    var formdata = [];
    var formdata = new FormData($('#providerProfile')[0]);
    formdata.append("selectedRegion", JSON.stringify(regionlist));
    formdata.append("IndependentContractorManagement", $('#IndependentContractorManagement')[0].files[0]);
    formdata.append("BackgroungCheck", $('#BackgroungCheck')[0].files[0]);
    formdata.append("HIPAA", $('#HIPAA')[0].files[0]);
    formdata.append("NondisclosureAggrement", $('#NondisclosureAggrement')[0].files[0]);
    formdata.append("photo", $('#photo')[0].files[0]);
    console.log(formdata);

    $.ajax({
        url: '/AdminProviders/CreateProvider',
        type: 'POST',
        data: formdata,
        processData: false,
        contentType: false,
        success: function (data) {
            loadPartialDashView('provider');
        },
        error: function () {
            console.error('Error loading partial view.');
        }
    });
}

function reminder() {
    $.ajax({
        url: '/AdminProviders/setreminder',
        type: 'GET',
        success: function (data) {
            console.log(1);
        },
        error: function () {
            console.error('Error loading partial view.');
        }
    });
}

