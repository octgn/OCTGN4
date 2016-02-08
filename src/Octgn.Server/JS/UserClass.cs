using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace Octgn.Server.JS
{
	public class UserClass : StateObject
    {
		public string id { get; protected set; }
        public string username { get; protected set; }
		private UserBase _user;

        internal UserClass(UserBase user, UserListClass parent)
            :base(user.Id.ToString(), parent)
        {
            _user = user;
            id = user.Id.ToString();
            username = user.Username;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if(binder.Name == "id")
            {
                result = id;
                return true;
            }
            if(binder.Name == "username")
            {
                result = username;
                return true;
            }
            return base.TryGetMember(binder, out result);
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            var list = base.GetDynamicMemberNames().ToList();
            list.Add("id");
            list.Add("username");
            return list;
        }
    }

    public class UserListClass : StateObject
    {
        internal UserListClass(StateClass cls)
            :base("users", cls)
        {
            IsArray = true;
        }

        internal void Add(UserClass user)
        {
            AddProperty(user.id.ToString(), user, true);
        }
    }
}
