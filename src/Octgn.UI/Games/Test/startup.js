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

O.state.chatLog = new ArrayStateObject();
O.state.chatLog = O.statefull([]);
// Array item added
O.state.chatLog.push(1);
O.state.chatLog.push(2);
// Array item removed
delete O.state.chatLog[0]
// Array item is changed
O.state.chatLog[0] = 12

O.state.someObj = new StateObject();
// Property is created
O.state.someObj.taco = 12;
O.state.someObj.taco2 = 12;
// Property is removed
delete O.state.someObj.taco;
// Property is changed
O.state.someObj.taco2 = 1;