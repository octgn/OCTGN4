using System;

namespace Octgn.Server
{
    public class GameServer : IDisposable
    {
        private GameEngine _engine;
        private GameServerListener _listener;
        public GameServer(int port, GameEngine engine)
        {
            _listener = new GameServerListener(port, OnSocket);
            _engine = engine;
        }

        private void OnSocket(GameServerSocket sock)
        {
            var user = new UnauthenticatedUser(sock);
            _engine.Users.AddUser(user);
        }

        public void Dispose()
        {
            _listener.Dispose();
        }
    }
}