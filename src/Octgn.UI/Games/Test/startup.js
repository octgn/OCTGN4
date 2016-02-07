O.com.on('user.authenticate', function (ctx) {
    if (ctd.user.id % 2) {
        ctx.allow = true;
    } else {
        ctx.allow = false;
    }
});

O.com.on('user.initialize', function (ctx) {
    ctx.layout = 'StartStage.html';
});

O.com.on('ping', function (obj) {
    O.com.broadcast('ping', obj);
});

//O.events.on('browser.opened', function (ctx) {
//	ctx.user.setLayout("StartStage.html");
//});