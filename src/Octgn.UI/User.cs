using System.Collections.Generic;
using System.Collections.Concurrent;
using Nancy.Security;
using System.Security.Principal;
using Octgn.UI.Gameplay;

namespace Octgn.UI
{
	public class User : IUserIdentity, IIdentity
	{
		public IEnumerable<string> Claims { get; } = new string[0];

		public string UserName { get; private set; }

		public string Sid { get; private set; }

		public string Name { get { return UserName; } }

		public string AuthenticationType { get { return "Session"; } }

		public bool IsAuthenticated { get { return true; } }

        public GameClient CurrentGameClient { get; set; }

		//public dynamic UIRPC { get; set; }

		private ConcurrentDictionary<int, GameClient> _clients;

		public User(string username, string sid)
		{
			UserName = username;
			Sid = sid;
			_clients = new ConcurrentDictionary<int, GameClient>();
		}

		public GameClient JoinGame(string host)
		{
			var client = new GameClient(host, this);
			_clients.AddOrUpdate(client.Id, client, (x, y) => client);
			return client;
		}

		public GameClient GetGame(int id)
		{
			return _clients[id];
		}
	}
}