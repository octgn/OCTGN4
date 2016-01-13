using Octgn.Shared.Networking;
using System.Net.Sockets;
using System;
using Octgn.Shared;

namespace Octgn.Server
{
    public class GameServerSocket : SocketBase
    {
        internal IGameServer _server;
        public GameServerSocket(IGameServer server, TcpClient sock)
            : base(sock)
        {
            _server = server;
        }
    }
}