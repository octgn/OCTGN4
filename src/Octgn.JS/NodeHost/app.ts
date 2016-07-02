import * as nconf from 'nconf';
import * as O from "./../Server/OServer";
import * as WebSocket from 'ws';
import {GameHost, HostConfig} from './GameHost';

console.log("App: Loading Config");
nconf.argv().env().file({ file: 'config.json' });

var config = new HostConfig();
config.Port = nconf.get("port");
config.GameId = nconf.get("game");

console.log("App: Creating host");
var host = new GameHost(config);
console.log("App: Starting host");
host.Start();

console.log('App: Server is running on port', host.Config.Port);

var socket = new WebSocket('ws://localhost:' + config.Port, {
    headers: { "username": "jim" }
});
socket.onerror = function (err: Error) {
    console.error(err);
}
socket.onopen = function () {
    console.log("App: Client connected");
    socket.onmessage = function (event: { data: any, type: string, target: WebSocket }) {
        console.log("App: client: " + event.data);
    }

    socket.send(JSON.stringify({ 'a': "b" }));
}