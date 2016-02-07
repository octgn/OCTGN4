using System;
using System.Collections.Generic;
using System.Dynamic;

namespace Octgn.Server.JS
{
    public class StateClass : StateObject
    {
        private GameEngine _engine;
        internal StateClass(string name, GameEngine engine)
            : base(name)
        {
            _engine = engine;
        }

        internal void AddUser(int id, string username)
        {
            var users = ((dynamic)this).users;
            users[id.ToString()] = $"{{id: {id}, username: {username}}}";
            //users[id.ToString()].id = id.ToString();
            //users[id.ToString()].username = username;
        }
        protected override void OnPropertyChanged(StateObject sender, string name, object val)
        {
            var id = _engine.StateHistory.StoreChange(name, val);
            if (id % 10 == 0)
                _engine.StateHistory.StoreFullState(id, this);
            _engine.Users.BroadcastRPC.StateChange(id, name, val.ToString());
        }
    }
}
