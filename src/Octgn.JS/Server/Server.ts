/// <reference path="./../Shared/Common.ts"/>
namespace O {
    export let Server: Octgn.Server = new Octgn.Server();
}

namespace Octgn {
    export type UserInitializeEventDelegate = (sender: any, args: UserInitializeEventArgs) => void;
    export type UserAuthenticateEventDelegate = (sender: any, args: UserAuthenticateEventArgs) => void;

    export class Server {
        public Com = new Communications();
        public Event = new Events();
        public State: any = {};
    }
}