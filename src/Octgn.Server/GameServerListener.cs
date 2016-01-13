using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Octgn.Server
{
    internal class GameServerListener : IDisposable
    {
        private bool _disposed;
        private TcpListener _listener;
        private Action<GameServerSocket> _onSocket;
        public GameServerListener(int port, Action<GameServerSocket> onSocket)
        {
            _listener = new TcpListener(System.Net.IPAddress.Any, port);
            _onSocket = onSocket;
            _listener.Start();
            Task.Factory.StartNew(Run, TaskCreationOptions.LongRunning);
        }

        private void Run()
        {
            while (!_disposed)
            {
                var socket = _listener.AcceptTcpClientAsync().Result;
                if (socket != null)
                {
                    // Handle the socket
                    _onSocket(new GameServerSocket(socket));
                }
            }
        }

        public void Dispose()
        {
            _disposed = true;
        }
    }
}