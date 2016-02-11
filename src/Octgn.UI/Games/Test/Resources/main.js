$(function () {
    O.on('com:msg', function () {
        $("#MessageList").append("<li>" + this.value + "</li>");
    });

    $("#btnSend").on('click', function () {
        var txt = $("#Message").val();
        O.com.send('msg', txt);
    });
});