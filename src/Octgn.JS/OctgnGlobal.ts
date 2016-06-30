/// <reference path="./Common.ts"/>
namespace Octgn {
    export type UserInitializeEventDelegate = (sender: any, args: UserInitializeEventArgs) => void;
    export type UserAuthenticateEventDelegate = (sender: any, args: UserAuthenticateEventArgs) => void;

    export class OctgnGlobal {
        public Com = new Communications();
        public Event = new Events();
        public State: any = {};
    }
}