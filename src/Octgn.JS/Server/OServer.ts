/// <reference path="./../typings/ws/ws.d.ts" />

import {Communications} from "./Communications";
import {Events} from "./Events";
import * as wsd from 'ws';

class OServer {
    public Com: Communications;
    public Event: Events;
    public State: any;

    private _server: wsd.Server;

    constructor() {
        this.Com = new Communications();
        this.Event = new Events();
        this.State = {};
    }

    public Start(port: number) {
        this._server = new wsd.Server({ port: port });

        this._server.on('connection', ws => {
            ws.on('message', message => {
                console.log(message);
            });
        });
    }
}

export let Server: OServer = new OServer();