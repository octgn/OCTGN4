using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace Octgn.Server.JS
{
	public class UserClass : DynamicObject
    {
		public string id { get; protected set; }
        public string username { get; protected set; }
		private UserBase _user;
        private GameEngine _engine;

        internal UserClass(GameEngine engine, UserBase user)
        {
            _engine = engine;
            _user = user;
            id = user.Id.ToString();
            username = user.Username;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if(binder.Name == nameof(id))
            {
                result = id;
                return true;
            }
            if(binder.Name == nameof(username))
            {
                result = username;
                return true;
            }
            return base.TryGetMember(binder, out result);
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            var list = base.GetDynamicMemberNames().ToList();
            list.Add(nameof(id));
            list.Add(nameof(username));
            return list;
        }
    }
}
