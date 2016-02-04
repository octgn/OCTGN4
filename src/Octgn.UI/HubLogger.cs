using Octgn.Shared;
using Octgn.UI.Hubs;
using System;
using System.Runtime.CompilerServices;

namespace Octgn.UI
{
	public class HubLogger : ILogger
	{
		private string _type;

		public void Setup(string type)
		{
			_type = type;
		}

		public void Debug(string message, params object[] args)
		{
			LoggingHub.Instance.Clients.All.debug(MakeString(message, args: args));
		}

		public void Error(string message, params object[] args)
		{
			LoggingHub.Instance.Clients.All.error(MakeString(message, args: args));
		}

		public void Standard(string message, params object[] args)
		{
			LoggingHub.Instance.Clients.All.standard(MakeString(message, args: args));
		}

		public void Trace(string message = "", [CallerMemberName] string caller = "", params object[] args)
		{
			LoggingHub.Instance.Clients.All.trace(MakeString($"{caller}() | " + message, args: args));
		}

		protected string MakeString(string str, [CallerMemberName] string cmem = "", params object[] args)
		{
			var strr = $"[{cmem.ToUpper()} {DateTime.Now}] - {string.Format(str, args)}";
			return strr;
		}
	}
}