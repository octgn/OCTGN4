using System;

namespace Octgn.Server.JS
{
	public class UserClass
	{
		public int id { get; set; }
		private UserBase _user;

		public UserClass(UserBase usr)
		{
			_user = usr;
			this.id = usr.Id;
		}

		public void setLayout(string layout)
		{
			_user.RPC.SetLayout(layout);
		}
	}
}
