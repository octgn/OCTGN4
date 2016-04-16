$(function () {
    $("#gamePort").text(O.com.port);
    O.on('com:msg', function () {
        $("#MessageList").append("<li>" + this.value + "</li>");
    });
    O.on('state:Update', function () {
        $('#MessageList').html("");
        for (var i = 0; i < O.state.chatLog.length; i++) {
            var cur = O.state.chatLog[i];
            $('#MessageList').Append("<li><b>[" + cur.from.username + "]</b> " + cur.message + "</li>");
        }
    });

    $("#btnSend").on('click', function () {
        var txt = $("#Message").val();
        O.com.send('msg', txt);
    });
});