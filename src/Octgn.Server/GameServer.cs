using Octgn.Shared.Networking;
using System;

namespace Octgn.Server
{
    public class GameServer : IDisposable
    {
        public int Id { get; private set; }
        public int Port { get; private set; }
        private static int _nextId = 0;
        private GameEngine _engine;
        private GameServerListener _listener;
        public GameServer(int port, GameEngine engine)
        {
            Port = port;
            _listener = new GameServerListener(Port, OnSocket);
            _engine = engine;
            Id = System.Threading.Interlocked.Increment(ref _nextId);
        }

        public GameServer(GameEngine engine)
            :this(SocketBase.FreeTcpPort(), engine)
        {

        }

        public GameServer()
            :this(new GameEngine())
        {

        }

        private void OnSocket(GameServerSocket sock)
        {
            var user = new UnauthenticatedUser(sock);
            _engine.Users.AddUser(user);
        }

        public void Dispose()
        {
            _listener.Dispose();
            _engine.Dispose();
        }
    }
}