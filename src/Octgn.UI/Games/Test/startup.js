O.com.on('ping', function (obj) {
    O.com.broadcast('ping', obj);
});

O.events.on('browser.opened', function (ctx) {
	ctx.user.setLayout("StartStage.html");
});