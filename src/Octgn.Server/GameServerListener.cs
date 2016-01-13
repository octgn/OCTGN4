using Octgn.Shared;
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
		private IGameServer _gs;
        public GameServerListener(int port, IGameServer gs, Action<GameServerSocket> onSocket)
        {
			_gs = gs;
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
                    // Need to be able to create this and send it back
                    _onSocket(new GameServerSocket(_gs, socket));
                }
            }
        }

        public void Dispose()
        {
            _disposed = true;
        }
    }
}