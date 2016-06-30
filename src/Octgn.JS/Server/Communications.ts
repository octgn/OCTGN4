import {Dict, Callback, NotImplementedException} from "./../Shared/Common";

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