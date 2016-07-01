import {Communications} from "./Communications";
import {Events} from "./Events";

class OServer {
    public Com: Communications;
    public Event: Events;
    public State: any;

    constructor() {
        this.Com = new Communications();
        this.Event = new Events();
        this.State = {};
    }
}

export let Server: OServer = new OServer();