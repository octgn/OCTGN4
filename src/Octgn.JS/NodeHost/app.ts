import * as nconf from 'nconf';
import * as O from "./../Server/OServer";
import {GameHost, HostConfig} from './GameHost';

nconf.argv().env().file({ file: 'config.json' });

var config = new HostConfig();
config.Port = nconf.get("port");
config.GameId = nconf.get("game");

var host = new GameHost(config);

console.log('Server is running on port', host.Config.Port);