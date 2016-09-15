$(function () {
    function RefreshChat() {
        $('#MessageList').html("");
        for (var property in O.state.chatLog) {
            var cur = O.state.chatLog[parseInt(property)];
            $('#MessageList').append("<li><b>[" + cur.from.username + "]</b> " + cur.message + "</li>");
        }
    }

    $("#gamePort").text(O.com.port);
    O.on('state:Updated', function () {
        RefreshChat();
    });

    $("#btnSend").on('click', function () {
        var txt = $("#Message").val();
        O.com.send('msg', txt);
    });
    RefreshChat();
});