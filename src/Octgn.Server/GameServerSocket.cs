using Octgn.Shared.Networking;
using System.Net.Sockets;
using System;

namespace Octgn.Server
{
    public class GameServerSocket : SocketBase
    {
        public GameServerSocket(TcpClient sock)
            : base(sock)
        {

        }
    }
}