using System;
using System.Net.Sockets;

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
            Run();
        }

        private async void Run()
        {
            _listener.Start();
            var socket = await _listener.AcceptTcpClientAsync().ConfigureAwait(false);
            if (socket != null)
            {
                // Handle the socket
                _onSocket(new GameServerSocket(socket));
            }

            lock (this)
            {
                if (_disposed)
                    return;
                Run();
            }
        }

        public void Dispose()
        {
            lock (this)
            {
                if (_disposed)
                    return;
                _disposed = true;
            }
        }
    }
}