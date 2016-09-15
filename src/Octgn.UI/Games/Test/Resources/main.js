$(function () {
    $("#gamePort").text(O.com.port);
    O.on('com:msg', function () {
        $("#MessageList").append("<li>" + this.value + "</li>");
    });
    O.on('state:Updated', function () {
        debugger;
        $('#MessageList').html("");
        for (var property in O.state.chatLog) {
            var cur = O.state.chatLog[property];
            $('#MessageList').append("<li><b>[" + cur.from.username + "]</b> " + cur.message + "</li>");
        }
    });

    $("#btnSend").on('click', function () {
        var txt = $("#Message").val();
        O.com.send('msg', txt);
    });
});