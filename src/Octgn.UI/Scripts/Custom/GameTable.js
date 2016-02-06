var O = O || {};
O.state = {};

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
    function fireCallback(name, obj) {
        var cb = callbacks[name];
        if (!cb) {
            return;
        }
        for (var i = 0; i < cb.length; i++) {
            cb[i](obj);
        }
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

		hub.on('firePropertyChanged', function (name, ob) {
            console.log("firePropertyChanged", {
                name: name,
                obj: obj
            });
            var parts = name.split('.');
            var curObj = O.state;
            var curProp = "";
            for (var i = 0; i < parts.length; i++) {
                curProp = parts[i];
                if (!curObj[curProp]) {
                    curObj[curProp] = {};
                }
                if (i < parts.length - 1)
                    curObj = curObj[curProp];
                else
                    curObj[curProp] = obj;
            }
		});

		this.Connection.reconnecting(function () {
		    fireCallback("con:reconnecting");
        });
		this.Connection.reconnected(function () {
		    fireCallback("con:reconnected");
        });

		this.Connection.disconnected(function () {
		    fireCallback("con:disconnected");
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
	        fireCallback("con:connected");
	    });
	}
}

O.com = new CommunicationsClass();
O.be = new BackendCommunicationClass();

O.init = function () {
    O.be.init();
    O.com.init();
}