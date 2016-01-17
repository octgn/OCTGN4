using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Octgn.Server
{
    internal class StateHistory
    {
        private ConcurrentDictionary<int, string> _changes;
        private int _nextId;

        internal StateHistory()
        {
            _changes = new ConcurrentDictionary<int, string>();
        }

        internal int StoreChange(string name, object val)
        {
            var id = Interlocked.Increment(ref _nextId);
            var change = JsonConvert.SerializeObject(val);
            change = name + "=" + change;
            _changes.AddOrUpdate(id, change, (x,y) => change);
            return id;
        }
    }
}
