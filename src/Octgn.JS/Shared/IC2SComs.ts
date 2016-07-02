export interface IC2SComs {
    RemoteCall(name: string, obj: any);
}

export interface IS2CComs {
    Kicked(message: string);
    RemoteCall(name: string, obj: any);
    StateChange(id: number, changes: any);
    FullState(id: number, state: any);
}