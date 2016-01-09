using Castle.DynamicProxy;
using Octgn.Shared.Networking;
using System;
using System.Net;
using System.Net.Sockets;

namespace Octgn.UI
{
    public class GameClient : IS2CComs, IDisposable
    {
        private static ProxyGenerator _generator = new ProxyGenerator();

        public IC2SComs RPC;
        private GameClientSocket _socket;

        public GameClient(int port)
        {
            var sock = new TcpClient();
            _socket = new GameClientSocket(sock);
            RPC = _generator.CreateInterfaceProxyWithoutTarget<IC2SComs>(new RpcInterceptor(_socket));
        }

        public void HelloResp(int id)
        {
            throw new NotImplementedException();
        }

        public void Kicked(string message)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            _socket.Dispose();
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
