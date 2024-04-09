function saveStopNotification(idlist, totallist) {
    $.ajax({
        url: '/DashActionView/' + actionType,
        type: 'POST',
        data: { idlist: idlist, totallist: totallist },
        success: function (data) {
            console.log("stop");
        },
        error: function () {
            console.error('Error loading partial view.');
        }
    });
}