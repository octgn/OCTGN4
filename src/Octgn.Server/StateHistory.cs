using Newtonsoft.Json;
using Octgn.Server.JS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Octgn.Server
{
    internal class StateHistory
    {
        private readonly Dictionary<int, StateChange> _changes;
        private readonly Dictionary<int, StateChange> _fullStateDump;
        private readonly ReaderWriterLockSlim _locker;
        private int _nextId = 0;

        internal StateHistory()
        {
            _locker = new ReaderWriterLockSlim();
            _changes = new Dictionary<int, StateChange>();
            _fullStateDump = new Dictionary<int, StateChange>();
        }

        internal int StoreChange(string name, object val)
        {
            try
            {
                _locker.EnterWriteLock();
                var id = Interlocked.Increment(ref _nextId);
                var change = JsonConvert.SerializeObject(val);
                var c = new StateChange(id, name, change);
                _changes.Add(id, c);
                return id;
            }
            finally
            {
                _locker.ExitWriteLock();
            }
        }

        internal void StoreFullState(int id, StateClass val)
        {
            try
            {
                _locker.EnterWriteLock();
                var change = JsonConvert.SerializeObject(val);
                var c = new StateChange(id, "%%FULL%%", change);
                _fullStateDump.Add(id, c);
            }
            finally
            {
                _locker.ExitWriteLock();
            }
        }

        internal IEnumerable<StateChange> GetLatestChanges()
        {
            try
            {
                _locker.EnterReadLock();
                var startNum = 1;
                if (_fullStateDump.Count > 0)
                {
                    var item = _fullStateDump.Last().Value;
                    startNum = item.Id + 1;
                    yield return item;
                }

                for(var i = startNum; i <= _changes.Count; i++)
                {
                    yield return _changes[i];
                }
            }
            finally
            {
                _locker.ExitReadLock();
            }
        }

        internal struct StateChange
        {
            public readonly int Id;
            public readonly string Name;
            public readonly string Change;
            public readonly DateTime DateCreated;
            public readonly bool IsFullState;

            private readonly string _toString;
            public StateChange(int id, string name, string change)
            {
                Id = id;
                Name = name;
                Change = change;
                DateCreated = DateTime.Now;
                IsFullState = name == "%%FULL%%";
                _toString = Id == 0 ? "" : $"{id}:{name}:{change}";
            }

            public override bool Equals(object obj)
            {
                if (obj == null) return false;
                if ((obj is StateChange) == false) return false;
                return Id == ((StateChange)obj).Id;
            }

            public override int GetHashCode()
            {
                return Id.GetHashCode();
            }

            public override string ToString()
            {
                return _toString;
            }
        }
    }
}
