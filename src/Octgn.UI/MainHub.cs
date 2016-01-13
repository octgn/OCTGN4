﻿using System;
using Microsoft.AspNet.SignalR;
using Octgn.Shared.Resources;
using System.Threading.Tasks;

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

        public string HostGame(string username)
        {
            try
            {
                var user = Context.User.Identity as User;
                var server = _locserver.LaunchServer();
                Task.Run(() => user.JoinGame(server.Id, server.Port));
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