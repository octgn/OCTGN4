import * as nconf from 'nconf';
import * as O from "./../Server/OServer";
import * as WebSocket from 'ws';
import {GameHost, HostConfig} from './GameHost';

console.log("Loading Config");
nconf.argv().env().file({ file: 'config.json' });

var config = new HostConfig();
config.Port = nconf.get("port");
config.GameId = nconf.get("game");

console.log("Creating host");
var host = new GameHost(config);
console.log("Starting host");
host.Start();

console.log('Server is running on port', host.Config.Port);

var socket = new WebSocket('ws://localhost:' + config.Port);
socket.onopen = function () {
    console.log("Client connected");
    socket.onmessage = function (mess) {
        console.log("client: " + mess);
    }

    socket.send(JSON.stringify({ 'a': "b" }));
}