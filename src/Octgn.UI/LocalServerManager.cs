using Octgn.Server;
using System;
using System.Collections.Concurrent;

namespace Octgn.UI
{
    public class LocalServerManager
    {
        private ConcurrentDictionary<int, GameServer> _servers;

        public LocalServerManager()
        {
            _servers = new ConcurrentDictionary<int, GameServer>();
        }

        public GameServer LaunchServer(string gameName)
        {
            var gs = new GameServer(gameName);

            _servers.AddOrUpdate(gs.Id, gs, (x, y) => gs);
            return gs;
        }

        public GameServer GetServer(int id)
        {
            GameServer ret = null;
            _servers.TryGetValue(id, out ret);
            return ret;
        }
    }
}