using System;
using System.Dynamic;

namespace Octgn.Server.JS
{
	public class EventContext
	{
		public UserClass user { get; set; }

		public EventContext(UserClass user)
		{
			this.user = user;
		}
	}
}
