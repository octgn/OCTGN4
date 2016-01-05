using Octgn.Server.Networking;
using System.Net.Sockets;
using System;

namespace Octgn.Server
{
    public class GameServerSocket : SocketBase
    {
        public GameServerSocket(TcpClient sock)
            : base(sock, new GameServerPacketInvoker())
        {

        }
    }

    public class GameServerPacketInvoker : IPacketInvoker
    {
        public void Invoke(NetworkProtocol.Packet packet)
        {
            throw new NotImplementedException();
        }
    }
}