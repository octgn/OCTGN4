var LogForwarderClass = function () {
	this.Hub = {};
	this.init = function () {
		setupSignalr();
	}

	function setupSignalr() {
		// Declare a proxy to reference the hub.
		this.Hub = $.connection.mainHub;

		//Set the hubs URL for the connection
		$.connection.hub.url = window.location.origin + "/signalr";
		if(window.location.search)
			$.connection.hub.qs = window.location.search.substr(1);

		this.Hub.client.trace = function(msg){
			console.log("[BE - Trace]: " + msg);
		}
		this.Hub.client.debug = function(msg){
			console.log("[BE - Debug]: " + msg);
		}
		this.Hub.client.standard = function(msg){
			console.log("[BE - Standard]: " + msg);
		}
		this.Hub.client.error = function(msg){
			setTimeout(function() {
				throw new Error("[BE - Error]: " + msg);
			}, 0);
		}

		// Start the connection.
		$.connection.hub.start().done(function () {
			$.connection.hub.reconnecting(function () {
				console.log("[BE]: Reconnecting...");
			});
			$.connection.hub.reconnected(function () {
				console.log("[BE]: Reconnected.");
			});

			$.connection.hub.disconnected(function () {
				console.log("[BE]: Disconnected.");
				setTimeout(function () {
					$.connection.hub.start();
				}, 5000); // Restart connection after 5 seconds.
				onDisconnected();
			});
			console.log("[BE]: Connected.");
		});	
	}
}