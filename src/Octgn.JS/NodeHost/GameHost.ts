/// <reference path='./../typings/node/node.d.ts' />
/// <reference path='./../typings/socket.io/socket.io.d.ts' />
import * as fs from 'fs';
import * as http from 'http';
import * as io from 'socket.io';
import * as cmn from './../Shared/Common';
import {GameHostBase} from './../Server/GameHostBase';
import {IHostConfig} from './../Server/IHostConfig';
import {User} from './../Server/User';
import {IUser} from './../Shared/IUser';

export class GameHost extends GameHostBase {
    private _server: http.Server;
    private _listener: SocketIO.Server;
    private _interop: SocketIO.Namespace;
    private _clients: cmn.Dict<SocketIO.Socket>;

    constructor(config: IHostConfig) {
        super(config);
        this._clients = {};
    }

    public Start() {
        if (this._interop) return;

        var so: SocketIO.ServerOptions = {
            allowRequest: (req: any, callback: (err: number, success: boolean) => void) => {
                var user = this.VerifyClient(req);
                callback(0, user != null);
            }
        };

        this._server = http.createServer();
        this._listener = io(this._server, so);

        var interop = this._listener.of('/interop');
        this._server.listen(this.Config.Port);

        interop.on('connection', socket => {
            var sessionId = socket.handshake.headers.sessionId;
            if (!sessionId) {
                socket.disconnect(true);
                return;
            }
            var user = this.Sessions[sessionId];
            if (!user) {
                socket.disconnect(true);
                return;
            }

            this._clients[user.Id] = socket;

            this.OnConnected(user);
            socket.on("disconnect", (): void => {
                this.OnDisconnected(user);
            });
            socket.on("event", (data): void => {
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
        for (var sessionId in this.Sessions) {
            this.Send(this.Sessions[sessionId], message);
        }
    }
}

export class HostConfig implements IHostConfig {
    public Port: number;
    public GameId: string;
}