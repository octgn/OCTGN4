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

        internal UserClass(GameEngine engine, UserBase user)
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
}
