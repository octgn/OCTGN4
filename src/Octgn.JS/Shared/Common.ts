export type Callback = (sender: any, obj: any) => void;

export interface Dict<TVal> {
    [K: string]: TVal;
}

export class NotImplementedException {

}