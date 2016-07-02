/// <reference path='./../typings/node/node.d.ts' />
/// <reference path='./../typings/ws/ws.d.ts' />
import * as wsd from 'ws';
import * as fs from 'fs';
import * as http from 'http';
import {GameHostBase} from './../Server/GameHostBase';
import {IHostConfig} from './../Server/IHostConfig';
import {User} from './../Server/User';
import {IUser} from './../Shared/IUser';

export class GameHost extends GameHostBase {
    private _server: wsd.Server;

    constructor(config: IHostConfig) {
        super(config);
    }

    public Start() {
        if (this._server) return;

        var serverConfig: wsd.IServerOptions = {
            port: this.Config.Port,
            verifyClient: (info: { origin: string; secure: boolean; req: http.ServerRequest }): boolean => {
                var user = this.VerifyClient(info.req);
                return (user != null);
            }
        };

        this._server = new wsd.Server(serverConfig);
        this._server.on('connection', ws => {
            var sessionId = ws.upgradeReq.headers.sessionId;
            if (!sessionId) {
                ws.close();
                return;
            }
            var user = this.Sessions[sessionId];
            if (!user) {
                ws.close();
                return;
            }

            this.OnConnected(user);
            ws.on("open", (): void => {
                this.OnConnected(user);
            });
            ws.on("close", (): void => {
                this.OnDisconnected(user);
            });
            ws.on("message", (data): void => {
                this.OnMessage(user, data);
            });
        });
    }
}

export class HostConfig implements IHostConfig {
    public Port: number;
    public GameId: string;
}