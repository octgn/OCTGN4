O.events.on('user.authenticate', function (ctx) {
    ctx.allow = true;
});

O.events.on('user.initialize', function (ctx) {
    O.state.users[ctx.user.id] = {
        layout: 'StartStage.html'
    };
});

O.com.on('msg', function (obj) {
    O.com.broadcast('msg', obj);
});