using Octgn.Server.Networking;
using System;
using System.Net.Sockets;

namespace Octgn.Client
{
    public class GameClient : IDisposable
    {
        private GameClientSocket _socket;
        public void Dispose()
        {
        }
    }

    class GameClientSocket : SocketBase
    {
        public GameClientSocket(TcpClient sock)
            : base(sock)
        {

        }
    }
}
