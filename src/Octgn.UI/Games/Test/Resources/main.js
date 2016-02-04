$(function () {
    O.com.on('ping', function (obj) {
        $("#MessageList").append("<li>" + obj + "</li>");
    });

    $("#btnSend").on('click', function () {
        var txt = $("#Message").val();
        O.com.send('ping', txt);
    });
});