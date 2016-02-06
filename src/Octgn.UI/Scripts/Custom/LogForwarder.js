var LogForwarderClass = function () {
	this.Hub = {};
	this.Connection = {};
	this.init = function () {
		setupSignalr();
	}

	function setupSignalr() {
		// Declare a proxy to reference the hub.
	    this.Connection = $.hubConnection("/signalr", { useDefaultPath: false });
	    this.Hub = this.Connection.createHubProxy('loggingHub');

		//Set the hubs URL for the connection
	    this.Connection.url = window.location.origin + "/signalr";
		if(window.location.search)
		    this.Connection.qs = window.location.search.substr(1);

		this.Hub.on('trace', function (msg) {
		    console.log("[BE - Trace]: " + msg);
		});
		this.Hub.on('debug', function (msg) {
		    console.log("[BE - Debug]: " + msg);
		});
		this.Hub.on('standard', function (msg) {
		    console.log("[BE - Standard]: " + msg);
		});
		this.Hub.on('error', function (msg) {
			setTimeout(function() {
				throw new Error("[BE - Error]: " + msg);
			}, 0);
		});

		this.Connection.reconnecting(function () {
            console.log("[BE]: Reconnecting...");
        });
		this.Connection.reconnected(function () {
            console.log("[BE]: Reconnected.");
        });

		this.Connection.disconnected(function () {
            console.log("[BE]: Disconnected.");
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
	        console.log("[BE]: Connected.");
	    });
	}
}