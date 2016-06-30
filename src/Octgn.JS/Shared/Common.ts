/// <reference path="./../typings/node/node.d.ts" />
export type Callback = (sender: any, obj: any) => void;

export interface Dict<TVal> {
    [K: string]: TVal;
}

export class NotImplementedException {

}

export interface Object {
    getName(): string;
}

Object.prototype.getName = function() {
    var funcNameRegex = /function (.{1,})\(/;
    var results = (funcNameRegex).exec((this).constructor.toString());
    return (results && results.length > 1) ? results[1] : "";
};