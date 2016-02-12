var OClass = function () {
    this.com = new CommunicationsClass();
    this.be = new BackendCommunicationClass();
    this.state = {};

    this.init = function () {
        O.be.init();
        O.com.init();
    }

    var callbacks = [];
    this.on = function (event, callback, filter) {
        if (!callbacks[event])
            callbacks[event] = [];
        var item = {
            name: event,
            callback: callback,
            filter: filter
        };
        callbacks[event].push(item);
        return item;
    }
    this.off = function (callback) {
        if (!callbacks[callback.name]) return;
        var idx = callbacks[callback.name].indexOf(callback.callback);
        if (idx == -1) return;
        callbacks[callback.name].splice(idx, 1);
    }
    this.fireOn = function (event, obj, filterable) {
        Object.keys(callbacks).reduce(function (prev, cur) {
            if (event != cur) return prev;
            for (var i = 0; i < callbacks[cur].length; i++) {
                var currentCallback = callbacks[cur][i];
                if (filterable && currentCallback.filter) {
                    var match = filterable.match(currentCallback.filter);
                    if (match) {
                        prev.push({
                            callback: currentCallback,
                            match: match
                        });
                    }
                } else {
                    prev.push({
                        callback: currentCallback
                    })
                }
            }
            return prev;
        }, []).forEach(function (cur) {
            var args = [];
            if (cur.match) {
                args = cur.match.slice(1);
            }
            var t = {
                name: cur.callback.name,
                value: obj
            };
            cur.callback.callback.apply(t, args)
        });
    }
}

var CommunicationsClass = function () {
    this.ConnectionStatus = 'disconnected';
    this.init = function () {
    };
    this.on = function (name, callback) {
    }
    this.send = function (name, obj) {
        O.be.invoke('Send', name, obj);
    }
    this.setConnectionStatus = function(status){
        O.com.ConnectionStatus = status;
        O.fireOn("connection:Changed", O.com, 'server:' + status);
    }
}

var BackendCommunicationClass = function () {
    var connection = {}
    var hub;
    var THIS = this;

    this.ConnectionStatus = 'disconnected';

    this.init = function () {
        setupSignalr();
    }
    this.invoke = function (name) {
        var args = [].slice.apply(arguments);
        return hub.invoke.apply(hub, args);
    }
    function setupSignalr() {
        // Declare a proxy to reference the hub.
        connection = $.hubConnection("/signalr", { useDefaultPath: false });
        hub = connection.createHubProxy('GameHub');

        //Set the hubs URL for the connection
        connection.url = window.location.origin + "/signalr";
        if (window.location.search)
            connection.qs = window.location.search.substr(1);

        hub.on('serverConnectionUpdated', function (status) {
            O.com.setConnectionStatus(status);
        });

        hub.on('fireStateReplaced', function (state) {
            O.state = JSON.parse(state);
            console.log("fireStateReplaced", state);
        });

        hub.on('firePropertyChanged', function (name, obj) {
            console.log("firePropertyChanged", {
                name: name,
                obj: obj
            });
            var fullName = 'O.state.' + name;
            var cur = fullName + ' = ' + JSON.stringify(obj);
            eval(cur);
            var obj = eval(fullName);
            O.fireOn('state:PropertyChanged', obj, name);
        });

        hub.on('loadCompleted', function () {
        });

        hub.on('invoke', function (name, obj) {
            O.fireOn('com:' + name, obj, name);
        });

        connection.reconnecting(function () {
            THIS.setConnectionStatus('connecting');
        });
        connection.reconnected(function () {
            THIS.setConnectionStatus('connected');
        });

        connection.disconnected(function () {
            THIS.setConnectionStatus('disconnected');
            setTimeout(function () {
                ConnectToSignalR();
            }, 5000); // Restart connection after 5 seconds.
        });

        // Start the connection.
        ConnectToSignalR();
    }

    function ConnectToSignalR() {
        THIS.setConnectionStatus('connecting');
        var obj = connection.start().done(function () {
            THIS.setConnectionStatus('connected');
        });
    }

    this.setConnectionStatus = function(status){
        THIS.ConnectionStatus = status;
        O.fireOn("connection:Changed", THIS, 'backend:' + status);
    }
}

var O = new OClass();