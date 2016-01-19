using System;

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
