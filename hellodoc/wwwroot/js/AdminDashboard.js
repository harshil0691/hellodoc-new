
$(document).ready(function () {
    $('#mydashboard a').on('shown.bs.tab', function (e) {
        var targetTab = $(e.target).attr('href').substring(1);
        loadPartialDashView(targetTab);
    });
    if (localStorage.getItem("DashTab") != null) {
        loadPartialDashView(localStorage.getItem("DashTab"), true);
    } else {
        loadPartialDashView("dashboard");
    }
});


function loadPartialDashView(tabId,ptab) {

    if (ptab == true) {
        tabId = localStorage.getItem("DashTab");
    }
    var element = document.querySelectorAll('[href="#' + tabId + '"]');
    element.forEach(a => {
        a.classList.add('active');
    });

    $.ajax({
        url: '/AdminDash/LoadPartialDashView',
        type: 'GET',
        data: { tabId: tabId },
        success: function (data) {
            $('#mainDashContent').html(data);

            if (tabId == 'records') {
                searchrecords('SearchRecords','1');
            }

            localStorage.setItem("DashTab", tabId);
        },
        error: function () {
            console.error('Error loading partial view.');
        }
    });
}

function loadPartialView(tabId, page, search, regionid) {

    var element = document.querySelectorAll('[href="#' + tabId + '"]');
    element.forEach(a => {
        a.classList.add('active');
    });

    $.ajax({
        url: '/LoadDashState/LoadPartialView',
        type: 'GET',
        data: { tabId: tabId, pagenumber: page, search: search, regionid: regionid },
        success: function (data) {
            $('#tabledata').html(data);
            localStorage.setItem("StatusTab", tabId);
        },
        error: function () {
            console.error('Error loading partial view.');
        }
    });

}

function exportToExcel(type) {

    var formdata = [];

    switch (type) {
        case "DashboardAll":
            formdata.push({ name: "exportType",value : "DashboardAll"});
            break;

        case "DashboardFilterd":
            formdata.push({ name: "exportType", value: "DashboardFilterd" });
            formdata.push({ name: "search", value: $('#customSearch').val() });
            formdata.push({ name: "regionid", value: $('#region123').val() });
            break;

        case "SearchRecords":
            formdata = $('#recordsForm').serializeArray();
            formdata.push({ name: "exportType", value: "SearchRecords" });
            break;
    }

    var form = $.param(formdata);

    $.ajax({
        url: '/AdminDash/exportToExcel',
        type: 'POST',
        data: form,
        responseType: 'blob',
        success: function (data) {
            var blob = new Blob([data], { type: 'application/octet-stream' });

            var url = window.URL.createObjectURL(blob);
            var link = document.createElement('a');
            link.href = url;
            link.download = 'downloaded_excel_file.xlsx';

            document.body.appendChild(link);
            link.click();

            document.body.removeChild(link);
            window.URL.revokeObjectURL(url);
        },
        error: function () {
            console.error('Error loading partial view.');
        }
    });

}