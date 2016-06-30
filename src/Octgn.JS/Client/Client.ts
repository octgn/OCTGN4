/// <reference path="./../Shared/Common.ts"/>
namespace O {
    let Client: Octgn.Client = new Octgn.Client();
}

namespace Octgn{
    export class Client {
        //public Com = new Communications();
        //public Event = new Events();
        public State: any = {};
    }
}
