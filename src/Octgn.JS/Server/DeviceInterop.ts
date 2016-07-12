/// <reference path='./../typings/socket.io-client/socket.io-client.d.ts' />
import * as io from 'socket.io-client';
export class DeviceInteropClient {
    private _port: number;
    private _socket: SocketIOClient.Socket;
    constructor(port: number) {
        this._port = port;
        this._socket = io.connect('http://127.0.0.1:' + this._port + '/interop');
    }


}