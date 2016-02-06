using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using Octgn.Shared;

namespace Octgn.UI.Hubs
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
            //TODO should probably have a hub per game, instead of piggy backing OR make sure to append the game id to calls that need it
			//user.UIRPC = this.Clients.Caller;
            user.UIRPC = this.Clients.User(user.UserName);
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

    public class GameHub : Hub
    {
		protected ILogger Log = LoggerFactory.Create<GameHub>();
        public static IHubContext Instance
        {
            get { return GlobalHost.ConnectionManager.GetHubContext<GameHub>(); }
        }

        public override Task OnConnected()
        {
            var user = Context.User.Identity as User;
            //TODO should probably have a hub per game, instead of piggy backing OR make sure to append the game id to calls that need it
            //user.UIRPC = this.Clients.Caller;
            user.UIRPC = this.Clients.User(user.UserName);
            if (user.GameClient != null)
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
}