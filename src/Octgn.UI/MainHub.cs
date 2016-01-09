using System;
using Microsoft.AspNet.SignalR;
using Octgn.Shared.Resources;

namespace Octgn.UI
{
    public class MainHub : Hub
    {
        public static IHubContext Instance
        {
            get { return GlobalHost.ConnectionManager.GetHubContext<MainHub>(); }
        }

        private LocalServerManager _locserver;

        public MainHub(LocalServerManager locserver)
        {
            _locserver = locserver;
        }

        public string HostGame(string username)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username))
                    throw new HubException(Text.MainHub_HostGame_UsernameValidationError);
                var server = _locserver.LaunchServer();
                return server.Id.ToString();
            }
            catch (System.Exception e)
            {
                if (e is HubException) throw;
                //TODO Log exception
                throw new HubException(Text.MainHub_HostGame_UnhandledError);
            }
        }
    }
}