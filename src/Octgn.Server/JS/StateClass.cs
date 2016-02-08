namespace Octgn.Server.JS
{
    public class StateClass : StateObject
    {
        private UserListClass _users;
        private GameEngine _engine;
        internal StateClass(string name, GameEngine engine)
            : base(name)
        {
            _engine = engine;
            _users = new UserListClass(this);
            AddProperty("users", _users, false);
        }

        internal UserClass AddUser(int id, string username)
        {
            var ub = this._engine.Users.Get(id);
            var uc = new UserClass(ub, _users);
            _users.Add(uc);
            return uc;
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
