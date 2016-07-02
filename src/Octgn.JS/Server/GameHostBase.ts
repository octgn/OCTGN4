import * as http from 'http';
import * as cmn from './../Shared/Common';
import {IHostConfig} from './../Server/IHostConfig';
import {IUser} from './../Shared/IUser';
import {User} from './../Server/User';

export abstract class GameHostBase {
    public Config: IHostConfig;

    // TODO This should be some kind of interface or something out to a non volatile cache.
    protected Sessions: cmn.Dict<User>;

    private _isRunning: boolean;

    constructor(protected config: IHostConfig) {
        this.Config = config;
    }

    public Start() {
        if (this._isRunning) return;
        this._isRunning = true;
    }

    protected VerifyClient(req: http.ServerRequest): IUser {
        if (!req.headers.username) return null;
        // TODO validate proper username here...actually at some point it will do username/password shit here
        var user = new User(req.headers.username);
        // TODO run the game user.authenticate event here, and return null if it fails or something?

        this.Sessions[user.SessionId] = user;
        return user;
    }

    protected OnConnected(user: IUser) {
        console.log("Host: OnConnected");
    }

    protected OnDisconnected(user: IUser) {
        console.log("Host: OnDisconnected");
    }

    protected OnMessage(user: IUser, message: string) {
        console.log("Host: GotMessage " + message);
    }
}