using Octgn.Shared;
using Octgn.Shared.Networking;
using System;

namespace Octgn.Server
{
    public class GameServer : IGameServer, IDisposable
    {
        public int Id { get; private set; }
        public int Port { get; private set; }
        public string Name { get; private set; }

        private static int _nextId = 0;
        private GameEngine _engine;
        private GameServerListener _listener;
        public GameServer(string name, int port, GameEngine engine)
        {
            Port = port;
            Name = name;
            _listener = new GameServerListener(Port, OnSocket);
            _engine = engine;
            Id = System.Threading.Interlocked.Increment(ref _nextId);
        }

        public GameServer(string name, GameEngine engine): this (name, SocketBase.FreeTcpPort(), engine)
        {
        }

        public GameServer(string name): this (name, new GameEngine())
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