/// <reference path="./../Shared/Common.ts"/>
/// <reference path="./../Shared/IUser.ts"/>

namespace Octgn {
    export class Events {
        private callbacks: Dict<Array<Callback>>;

        OnUserInitialize(callback: UserInitializeEventDelegate) {
            let name = callback.getName();
            if (!(name in this.callbacks)) {
                this.callbacks[name] = new Array<UserInitializeEventDelegate>();
            }
            this.callbacks[name].push(callback);
        }

        OnUserAuthenticate(callback: UserAuthenticateEventDelegate) {
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
}