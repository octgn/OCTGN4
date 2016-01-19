using System;
using Microsoft.AspNet.SignalR;
using Octgn.Shared.Resources;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Octgn.Shared;

namespace Octgn.UI
{
    public class MainHub : Hub
    {
		protected ILogger Log = LoggerFactory.Create<MainHub>();
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
				Log.Error(e.ToString());
                if (e is HubException) throw;
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
				Log.Error(e.ToString());
                throw new HubException(Text.MainHub_HostGame_UnhandledError);
            }
        }

		public void Send(string name, object o)
		{
			var user = Context.User.Identity as User;
			user.GameClient.RPC.RemoteCall(name, o);
		}

        public void BrowserOpened()
        {
			var user = Context.User.Identity as User;
            user.GameClient.RPC.BrowserOpened();
        }

		public override Task OnConnected()
		{
			var user = Context.User.Identity as User;
            //TODO Update this to allow for multiple windows with the same user
            //TODO should probably have a hub per game, instead of piggy backing OR make sure to append the game id to calls that need it
			user.UIRPC = this.Clients.Caller;
            if(user.GameClient != null)
                user.GameClient.SendStateToUI();

            return base.OnConnected();
		}

		public override Task OnDisconnected(bool stopCalled)
		{
			var user = Context.User.Identity as User;
			user.UIRPC = null;
			return base.OnDisconnected(stopCalled);
		}
	}

	public class LoggingHub : Hub
	{
        public static IHubContext Instance
        {
            get { return GlobalHost.ConnectionManager.GetHubContext<LoggingHub>(); }
        }
	}
}