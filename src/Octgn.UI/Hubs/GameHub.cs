using Microsoft.AspNet.SignalR;
using Octgn.Shared;

namespace Octgn.UI.Hubs
{
    public class GameHub : Hub
    {
        protected ILogger Log = LoggerFactory.Create<GameHub>();
    }
}