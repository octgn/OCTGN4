O.com.on('ping', function (obj) {
    O.com.broadcast('ping', obj);
});

O.state.chicken = 12;
O.state.chicken2 = {
    taco: 12
};