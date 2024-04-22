$(document).ready(function () {
    $('#mydashboard123 a').on('shown.bs.tab', function (e) {
        var targetTab = $(e.target).attr('href').substring(1);
        console.log(1);
        loadPartialDashView(targetTab);
    });
    console.log(1);
    loadPartialDashView("dashboard");
});

function loadPartialDashView(tabId, ptab) {

    var element = document.querySelectorAll('[href="#' + tabId + '"]');
    element.forEach(a => {
        a.classList.add('active');
    });

    $.ajax({
        url: '/ProviderDashboard/LoadPartialDashView',
        type: 'GET',
        data: { tabId: tabId },
        success: function (data) {
            $('#mainDashContent').html(data);

            if (tabId == "scheduling") {
                ShiftCalender("month");
            }

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
        data: { tabId: tabId, pageNumber: page, search: search, regionid: regionid, provider: true },
        success: function (data) {
            $('#tabledata').html(data);
            localStorage.setItem("StatusTab", tabId);
        },
        error: function () {
            console.error('Error loading partial view.');
        }
    });

}