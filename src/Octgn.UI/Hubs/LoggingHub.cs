using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNet.SignalR;

namespace Octgn.UI.Hubs
{
    public class LoggingHub : Hub
    {
        public static IHubContext Instance
        {
            get { return GlobalHost.ConnectionManager.GetHubContext<LoggingHub>(); }
        }
    }
}