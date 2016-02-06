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

        public void GameStatusUpdated(bool connected)
        {
            dynamic[] cs = new dynamic[0];
            lock (_clients)
            {
                cs = _clients.ToArray();
            }
            foreach(var c in cs)
            {
                c.GameStatusUpdated(connected);
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
                c.Invoke(name, obj);
            }
        }

        public void FireSetLayout(string layout)
        {
            dynamic[] cs = new dynamic[0];
            lock (_clients)
            {
                cs = _clients.ToArray();
            }
            foreach(var c in cs)
            {
                c.FireSetLayout(layout);
            }
        }

        public void FirePropertyChanged(string name, object realo)
        {
            dynamic[] cs = new dynamic[0];
            lock (_clients)
            {
                cs = _clients.ToArray();
            }
            foreach(var c in cs)
            {
                c.FirePropertyChanged(name, realo);
            }
        }

        public void FireStateReplaced(string state)
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