using Octgn.Server;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Reflection;

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
            //var bp = new FileInfo(typeof(LocalServerManager).Assembly.Location).Directory.FullName;
            //var bp = Server.MapPath("/");
            var bp = AppDomain.CurrentDomain.BaseDirectory;
            bp = Path.Combine(bp, "Games\\Test");
            var rp = new GameResourceProvider(bp);
            var gs = new GameServer(gameName, rp);

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