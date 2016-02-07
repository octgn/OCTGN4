O.events.on('user.authenticate', function (ctx) {
    ctx.allow = true;
});

O.events.on('user.initialize', function (ctx) {
    O.state.users[ctx.user.id] = {
        layout: 'StartStage.html'
    };
});

O.com.on('ping', function (obj) {
    O.com.broadcast('ping', obj);
});