import {IUser} from "./../Shared/IUser";
import {Dict, Callback, NotImplementedException} from "./../Shared/Common";
import {GameHostBase} from "./GameHostBase";

export type UserInitializeEventDelegate = (sender: any, args: UserInitializeEventArgs) => void;
export type UserAuthenticateEventDelegate = (sender: any, args: UserAuthenticateEventArgs) => void;
export class Events {
    private callbacks: Dict<Array<Callback>>;
    private _host: GameHostBase;

    //constructor(host: GameHostBase) {
    //}

    public OnUserInitialize(callback: UserInitializeEventDelegate) {
        let name = callback.getName();
        if (!(name in this.callbacks)) {
            this.callbacks[name] = new Array<UserInitializeEventDelegate>();
        }
        this.callbacks[name].push(callback);
    }

    public OnUserAuthenticate(callback: UserAuthenticateEventDelegate) {
        let name = callback.getName();
        if (!(name in this.callbacks)) {
            this.callbacks[name] = new Array<UserInitializeEventDelegate>();
        }
        this.callbacks[name].push(callback);
    }
}
export class UserInitializeEventArgs {
    public User: IUser;
    public Allow: Boolean = true;
}
export class UserAuthenticateEventArgs {
    public User: IUser;
}