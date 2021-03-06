using Octgn.Shared;
using System;
using System.Collections.Generic;

namespace Octgn.UI
{
    public class GameUIRPC
    {
        private List<dynamic> _clients;

        public GameUIRPC()
        {
            _clients = new List<dynamic>();
        }

        public void ServerConnectionUpdated(string val)
        {
            dynamic[] cs = new dynamic[0];
            lock (_clients)
            {
                cs = _clients.ToArray();
            }
            foreach(var c in cs)
            {
                c.serverConnectionUpdated(val);
            }
        }

        public void Invoke(string name, object obj)
        {
            dynamic[] cs = new dynamic[0];
            lock (_clients)
            {
                cs = _clients.ToArray();
            }
            foreach(var c in cs)
            {
                c.invoke(name, obj);
            }
        }

        internal void LoadCompleted()
        {
            dynamic[] cs = new dynamic[0];
            lock (_clients)
            {
                cs = _clients.ToArray();
            }
            foreach(var c in cs)
            {
                c.LoadCompleted();
            }
        }

        public void FireStateUpdated(ObjectDiff obj)
        {
            dynamic[] cs = new dynamic[0];
            lock (_clients)
            {
                cs = _clients.ToArray();
            }
            foreach(var c in cs)
            {
                c.FireStateUpdated(obj);
            }
        }

        public void FireStateReplaced(object state)
        {
            dynamic[] cs = new dynamic[0];
            lock (_clients)
            {
                cs = _clients.ToArray();
            }
            foreach(var c in cs)
            {
                c.FireStateReplaced(state);
            }
        }

        internal void AddConnection(dynamic caller)
        {
            lock (_clients)
            {
                _clients.Add(caller);
            }
        }

        internal void RemoveConnection(dynamic caller)
        {
            lock (_clients)
            {
                _clients.Remove(caller);
            }
        }
    }
}