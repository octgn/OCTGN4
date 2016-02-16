namespace Octgn.Server.JS
{
    public class StateClass : StatefullObject
    {
        private UserListClass _users;
        internal StateClass(GameEngine engine)
            : base("O.state", engine, null)
        {
            _users = new UserListClass(engine, this);
            dynamicObject.users = _users;
        }

        internal UserClass AddUser(int id, string username)
        {
            var ub = this.Engine.Users.Get(id);
            var uc = new UserClass(Engine, ub, _users);
            _users.Add(uc);
            return uc;
        }

        protected override void OnPropertyChanged(StatefullObject sender, PropertyChangedEventArgs args)
        {
            var cname = sender.FullName + "." + args.PropertyName;
            var id = Engine.StateHistory.StoreChange(cname, args.Value);
            if (id % 10 == 0)
                Engine.StateHistory.StoreFullState(id, this);
            Engine.Users.BroadcastRPC.StateChange(id, cname, args.Value.ToString());
        }
    }
}
