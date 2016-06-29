/// <reference path="./Common.ts"/>

namespace Octgn {
    export class Communications {
        private callbacks: Dict<Array<Callback>>;
        on(name: string, callback: Callback) {
            if (!(name in this.callbacks)) {
                this.callbacks[name] = new Array<Callback>();
            }
            this.callbacks[name].push(callback);
        }

        fire_on(name: string, obj: any) {
            if (!(name in this.callbacks)) return;
            this.callbacks[name].forEach((cb, idx, list) => {
                cb(this, obj);
            });
        }

        broadcast(name: string, obj: any) {
            throw new NotImplementedException();
        }
    }

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
        public User: User;
        public Allow: Boolean = true;
    }
    export class UserAuthenticateEventArgs {
        public User: User;
    }
}