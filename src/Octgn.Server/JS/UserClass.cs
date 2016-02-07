using System;

namespace Octgn.Server.JS
{
	public class UserClass
	{
		public string id { get; set; }
        public string Username { get; set; }
		private UserBase _user;

		public UserClass(UserBase usr)
		{
			_user = usr;
			this.id = usr.Id.ToString();
            this.Username = usr.Username;
		}

		public void setLayout(string layout)
		{
			_user.RPC.SetLayout(layout);
		}
	}
}
