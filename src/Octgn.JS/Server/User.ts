import * as uuid from 'node-uuid';
import {IUser} from './../Shared/IUser';

export class User implements IUser{
    public Id: number;
    public Username: string;
    public SessionId: string;

    private static _nextId: number = 0;

    constructor(username: string) {
        this.Id = User._nextId++;
        this.Username = username;
        this.SessionId = uuid.v4();
    }

    public RemoteCall(name: string, obj: any) {
        // TODO do remote call.
    }
}