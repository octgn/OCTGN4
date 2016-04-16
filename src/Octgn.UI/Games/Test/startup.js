O.events.on('user.authenticate', function (ctx, obj) {
    obj.allow = true;
});

O.events.on('user.initialize', function (ctx) {
    O.state.users[ctx.user.id].layout = "StartStage.html";
});

O.com.on('msg', function (ctx, obj) {
    var msg = {
        from: ctx.user,
        time: new Date().getTime(),
        message: obj
    };
    O.state.chatLog.push(msg);
});

O.state.chatLog = [];