
$(document).ready(function () {
    $('#mydashboard a').on('shown.bs.tab', function (e) {
        var targetTab = $(e.target).attr('href').substring(1);
        loadPartialDashView(targetTab);
    });
    loadPartialDashView("dashboard");
});


function loadPartialDashView(tabId) {
    $.ajax({
        url: '/AdminDash/LoadPartialDashView',
        type: 'GET',
        data: { tabId: tabId },
        success: function (data) {
            $('#mainDashContent').html(data);

            if (tabId == 'records') {
                searchrecords('SearchRecords','1');
            }

        },
        error: function () {
            console.error('Error loading partial view.');
        }
    });
}

function loadPartialView(tabId, page, search, regionid) {
    $.ajax({
        url: '/LoadDashState/LoadPartialView',
        type: 'GET',
        data: { tabId: tabId, pagenumber: page, search: search, regionid: regionid },
        success: function (data) {
            $('#tabledata').html(data);
            
        },
        error: function () {
            console.error('Error loading partial view.');
        }
    });

}
