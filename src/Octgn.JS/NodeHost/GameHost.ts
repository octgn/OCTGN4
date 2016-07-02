/// <reference path='./../typings/node/node.d.ts' />
/// <reference path='./../typings/ws/ws.d.ts' 6/>
import * as wsd from 'ws';
import * as fs from 'fs';
import {IGameHost} from './../Server/IGameHost';
import {IHostConfig} from './../Server/IHostConfig';

export class GameHost implements IGameHost {
    public Config: IHostConfig;
    private _server: wsd.Server;

    constructor(config: IHostConfig) {
        this.Config = config;
    }

    public Start() {
        if (this._server) return;

        this._server = new wsd.Server({ port: this.Config.Port });

        this._server.on('connection', ws => {
            ws.on("open", (): void => {
                console.log("Connection opened", this);
            });
            ws.on('message', message => {
                console.log(message);
            });
        });
    }
}

export class HostConfig implements IHostConfig {
    public Port: number;
    public GameId: string;
}