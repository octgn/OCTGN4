type Callback = (sender: any, obj: any) => void;

interface Dict<TVal> {
    [K: string]: TVal;
}

class NotImplementedException {
    
}

interface Object {
    getName(): string;
}

Object.prototype.getName = (): string => {
    var funcNameRegex = /function (.{1,})\(/;
    var results = (funcNameRegex).exec((this).constructor.toString());
    return (results && results.length > 1) ? results[1] : "";
};