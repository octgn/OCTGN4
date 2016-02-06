using Microsoft.AspNet.SignalR;
using Octgn.Shared;
using System.Threading.Tasks;

namespace Octgn.UI.Hubs
{
    public class GameHub : Hub
    {
        protected ILogger Log = LoggerFactory.Create<GameHub>();
        public static IHubContext Instance
        {
            get { return GlobalHost.ConnectionManager.GetHubContext<GameHub>(); }
        }

        public void Send(string name, object o)
        {
            var user = Context.User.Identity as User;
            user.CurrentGameClient.RPC.RemoteCall(name, o);
        }

        public override Task OnConnected()
        {
            var user = Context.User.Identity as User;
            var gc = user.CurrentGameClient;

            gc.UIRPC.AddConnection(this.Clients.Caller);

            gc.SendStateToUI();

            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            var user = Context.User.Identity as User;
            var gc = user.CurrentGameClient;

            gc.UIRPC.RemoveConnection(this.Clients.Caller);

            return base.OnDisconnected(stopCalled);
        }
    }
}