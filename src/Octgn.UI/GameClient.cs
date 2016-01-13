using Castle.DynamicProxy;
using Octgn.Shared;
using Octgn.Shared.Networking;
using System;
using System.Net;
using System.Net.Sockets;

namespace Octgn.UI
{
    public class GameClient : IS2CComs, IDisposable
    {
        private static ProxyGenerator _generator = new ProxyGenerator();

        public IC2SComs RPC { get; private set; }
        private GameClientSocket _socket;
        private User _user;
        private Action<GameClient, IGameServer> _onConnect;

        public GameClient(string host, User user)
        {
            _user = user;
            var sock = new TcpClient();
            _socket = new GameClientSocket(sock, host);
            RPC = _generator.CreateInterfaceProxyWithoutTarget<IC2SComs>(new RpcInterceptor(_socket));
        }

        public void Connect(Action<GameClient, IGameServer> onConnect)
        {
            _onConnect = onConnect;
            _socket.Connect();
        }

        public void HelloResp(IGameServer server)
        {
            _onConnect(this, server);
        }

        public void Kicked(string message)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            _socket.Dispose();
        }

        class GameClientSocket : SocketBase
        {
            private TcpClient _socket;
            private string _host;
            public GameClientSocket(TcpClient sock, string host)
                : base(sock)
            {
                _socket = sock;
                _host = host;
            }

            public void Connect()
            {
                var endpoint = ParseIPEndPoint(_host);
                _socket.Connect(endpoint);
            }

            private static IPEndPoint ParseIPEndPoint(string text)
            {
                Uri uri;
                if (Uri.TryCreate(text, UriKind.Absolute, out uri))
                    return new IPEndPoint(IPAddress.Parse(uri.Host), uri.Port < 0 ? 0 : uri.Port);
                if (Uri.TryCreate(String.Concat("tcp://", text), UriKind.Absolute, out uri))
                    return new IPEndPoint(IPAddress.Parse(uri.Host), uri.Port < 0 ? 0 : uri.Port);
                if (Uri.TryCreate(String.Concat("tcp://", String.Concat("[", text, "]")), UriKind.Absolute, out uri))
                    return new IPEndPoint(IPAddress.Parse(uri.Host), uri.Port < 0 ? 0 : uri.Port);
                throw new FormatException("Failed to parse text to IPEndPoint");
            }
        }
    }
}
