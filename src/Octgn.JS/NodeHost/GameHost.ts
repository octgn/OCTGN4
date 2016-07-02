/// <reference path='./../typings/node/node.d.ts' />
/// <reference path='./../typings/ws/ws.d.ts' />
import * as WebSocket from 'ws';
import * as fs from 'fs';
import * as http from 'http';
import * as cmn from './../Shared/Common';
import {GameHostBase} from './../Server/GameHostBase';
import {IHostConfig} from './../Server/IHostConfig';
import {User} from './../Server/User';
import {IUser} from './../Shared/IUser';

export class GameHost extends GameHostBase {
    private _server: WebSocket.Server;
    private _clients: cmn.Dict<WebSocket>;

    constructor(config: IHostConfig) {
        super(config);
        this._clients = {};
    }

    public Start() {
        if (this._server) return;

        var serverConfig: WebSocket.IServerOptions = {
            port: this.Config.Port,
            verifyClient: (info: { origin: string; secure: boolean; req: http.ServerRequest }): boolean => {
                var user = this.VerifyClient(info.req);
                return (user != null);
            }            
        };

        this._server = new WebSocket.Server(serverConfig);
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

            this._clients[user.Id] = ws;

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

    public Send(user: IUser, message: string) {
        var client = this._clients[user.Id];
        if (!client) return;

        client.send(message);
    }

    public Broadcast(message: string) {
        this._server.clients.forEach((item, idx, arr) => {
            if (!item.upgradeReq.headers.sessionId) return;

            var user = this.Sessions[item.upgradeReq.headers.sessionId];
            this.Send(user, message);
        });
    }
}

export class HostConfig implements IHostConfig {
    public Port: number;
    public GameId: string;
}