var CommunicationsClass = function () {
	var callbacks = [];
	this.init = function() {
		gHub.client.invoke = function (name, obj) {
			var cb = callbacks[name];
			if (!cb) {
				console.log("Missed call '" + name + "' because no listeners were defined.");
				return;
			}
			for (var i = 0; i < cb.length; i++) {
				cb[i](obj);
			}
		}
	}
	this.on = function (name, callback) {
		if (!callbacks[name])
			callbacks[name] = [];
		callbacks[name].push(callback);
	}
	this.send = function (name, obj) {
		gHub.server.send(name, obj);
	}
}

O.com = new CommunicationsClass();