using System;
using Microsoft.AspNet.SignalR;
using Octgn.Shared.Resources;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Octgn.UI
{
    public class MainHub : Hub
    {
        public static IHubContext Instance
        {
            get { return GlobalHost.ConnectionManager.GetHubContext<MainHub>(); }
        }

        private LocalServerManager _locserver;
        private UserSessions _sessions;

        public MainHub(LocalServerManager locserver, UserSessions sessions)
        {
            _locserver = locserver;
            _sessions = sessions;
        }

        public int HostGame(string gameName)
        {
            try
            {
                if(!Regex.IsMatch(gameName, @"^[a-zA-Z0-9]+[a-zA-Z0-9\-_ ]+$"))
                    throw new HubException(Text.MainHub_GameNameInvalid);
                var user = Context.User.Identity as User;
                var server = _locserver.LaunchServer(gameName);
                var client = user.JoinGame("localhost:" + server.Port);
				Task.Run(()=>client.Connect());
				return client.Id;
            }
            catch (System.Exception e)
            {
                if (e is HubException) throw;
                //TODO Log exception
                throw new HubException(Text.MainHub_HostGame_UnhandledError);
            }
        }

        public int JoinGame(string host)
        {
            try
            {
                var user = Context.User.Identity as User;
                var client = user.JoinGame(host);
				Task.Run(()=>client.Connect());
				return client.Id;
            }
            catch (System.Exception e)
            {
                if (e is HubException) throw;
                //TODO Log exception
                throw new HubException(Text.MainHub_HostGame_UnhandledError);
            }
        }

		public override Task OnConnected()
		{
			var user = Context.User.Identity as User;
			user.UIRPC = this.Clients.Caller;
			return base.OnConnected();
		}

		public override Task OnDisconnected(bool stopCalled)
		{
			var user = Context.User.Identity as User;
			user.UIRPC = null;
			return base.OnDisconnected(stopCalled);
		}
	}
}