using System;
using System.Dynamic;

namespace Octgn.Server.JS
{
	public class EventContext
	{
		public dynamic user { get; set; }

		public EventContext(dynamic user)
		{
			this.user = user;
		}
	}
}
