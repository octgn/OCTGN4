using Octgn.Shared;
using System.Collections.Generic;
using System.Dynamic;

namespace Octgn.Server.JS
{
    public class StateClass : DynamicObject
    {
        internal ChangeTracker ChangeTracker { get; private set; }

        private Dictionary<string, object> _properties;
        private Dictionary<int, UserClass> _users;
        private GameEngine _engine;
        private object _previous;
        internal StateClass(GameEngine engine)
        {
            _engine = engine;
            _users = new Dictionary<int, UserClass>();
            _properties = new Dictionary<string, object>();
            _previous = new object();
            ChangeTracker = engine.CreateChangeTracker(this);
        }

        internal UserClass AddUser(int id, string username)
        {
            var ub = _engine.Users.Get(id);
            var uc = new UserClass(_engine, ub);
            _users.Add(id, uc);
            return uc;
        }

        #region Dynamic
        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return _properties.Keys;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            return _properties.TryGetValue(binder.Name, out result);
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            _properties[binder.Name] = value;
            return true;
        }

        public override bool TryDeleteMember(DeleteMemberBinder binder)
        {
            return _properties.Remove(binder.Name);
        }
        #endregion
    }
}
