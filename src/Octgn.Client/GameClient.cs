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

    class GameClientSocket : SocketBase, IPacketInvoker
    {
        public GameClientSocket(TcpClient sock)
            : base(sock, new GameClientPacketInvoker())
        {

        }

        public void Invoke(NetworkProtocol.Packet packet)
        {
            throw new NotImplementedException();
        }
    }

    public class GameClientPacketInvoker : IPacketInvoker
    {
        public void Invoke(NetworkProtocol.Packet packet)
        {
            throw new NotImplementedException();
        }
    }
}
