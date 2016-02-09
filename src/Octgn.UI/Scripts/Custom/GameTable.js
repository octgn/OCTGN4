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
	var callbacks = [];
	this.init = function() {
	    O.be.on('invoke', function (name, obj) {
	        var cb = callbacks[name];
	        if (!cb) {
	            console.log("Missed call '" + name + "' because no listeners were defined.");
	            return;
	        }
	        for (var i = 0; i < cb.length; i++) {
	            cb[i](obj);
	        }
	    });
	};
	this.on = function (name, callback) {
		if (!callbacks[name])
			callbacks[name] = [];
		callbacks[name].push(callback);
	}
	this.send = function (name, obj) {
	    O.be.invoke('Send', name, obj);
	}
}

var BackendCommunicationClass = function () {
    var callbacks = [];
    var srcallbacks = [];
    this.Connection = {}
    var hub;
    this.init = function () {
        setupSignalr();
    }
    this.on = function (name, callback) {
        if (name.indexOf('con:') === 0) {
            if (!callbacks[name])
                callbacks[name] = [];
            callbacks[name].push(callback);
        } else {
            if (hub)
                hub.on(name, callback);
            else {
                if (!srcallbacks[name])
                    srcallbacks[name] = [];
                srcallbacks[name].push(callback);
            }
        }
    }
    this.invoke = function (name) {
        var args = [].slice.apply(arguments);
        return hub.invoke.apply(hub, args);
    }
	function setupSignalr() {
		// Declare a proxy to reference the hub.
	    this.Connection = $.hubConnection("/signalr", { useDefaultPath: false });
	    hub = this.Connection.createHubProxy('GameHub');

		//Set the hubs URL for the connection
	    this.Connection.url = window.location.origin + "/signalr";
		if(window.location.search)
		    this.Connection.qs = window.location.search.substr(1);

		Object.keys(srcallbacks).forEach(function (key) {
		    var cbs = srcallbacks[key]
		    for (var i = 0; i < cbs.length; i++) {
		        hub.on(key, cbs[i]);
		    }
		})
		delete srcallbacks;

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

		this.Connection.reconnecting(function () {
		    O.fireOn("connection:StatusChanged", 'reconnecting', 'reconnecting');
        });
		this.Connection.reconnected(function () {
		    O.fireOn("connection:StatusChanged", 'reconnected', 'reconnected');
        });

		this.Connection.disconnected(function () {
		    O.fireOn("connection:StatusChanged", 'disconnected', 'disconnected');
            setTimeout(function () {
                ConnectToSignalR();
            }, 5000); // Restart connection after 5 seconds.
        });

	    // Start the connection.
		ConnectToSignalR();
	}

	function ConnectToSignalR() {
	    var connection = this.Connection;
	    var obj = this.Connection.start().done(function () {
	        console.log("Connected to Game Backend");
	        O.fireOn("connection:StatusChanged", 'connected', 'connected');
	    });
	}
}

var O = new OClass();