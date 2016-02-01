using Octgn.Server;
using Octgn.Shared;
using System;
using System.Collections.Concurrent;
using System.IO;

namespace Octgn.UI
{
    public class LocalServerManager
    {
        private ConcurrentDictionary<int, GameServer> _servers;

        public LocalServerManager()
        {
            _servers = new ConcurrentDictionary<int, GameServer>();
        }

        public IGameServer LaunchServer(string gameName)
        {
            var bp = AppDomain.CurrentDomain.BaseDirectory;
            bp = Path.Combine(bp, "Games\\Test");
            var rp = new GameResourceProvider(bp);
            var gs = new GameServer(gameName, rp);

            _servers.AddOrUpdate(gs.Id, gs, (x, y) => gs);
            return gs;
        }

        public IGameServer GetServer(int id)
        {
            GameServer ret = null;
            _servers.TryGetValue(id, out ret);
            return ret;
        }
    }
}