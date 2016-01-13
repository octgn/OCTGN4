using System.Collections.Generic;
using System.Collections.Concurrent;
using Nancy.Security;
using System.Security.Principal;
using Octgn.Shared;
using System;

namespace Octgn.UI
{
    public class User : IUserIdentity, IIdentity
    {
        public IEnumerable<string> Claims { get; } = new string[0];

        public string UserName { get; private set; }

        public string Sid { get; private set; }

        public string Name
        {
            get
            {
                return UserName;
            }
        }

        public string AuthenticationType
        {
            get
            {
                return "Session";
            }
        }

        public bool IsAuthenticated
        {
            get
            {
                return true;
            }
        }

        private ConcurrentDictionary<int, GameClient> _clients;

        public User(string username, string sid)
        {
            UserName = username;
            Sid = sid;
            _clients = new ConcurrentDictionary<int, GameClient>();
        }

        public void JoinGame(string host)
        {
            var client = new GameClient(host, this);
            client.Connect(OnJoinGame);
        }

        private void OnJoinGame(GameClient sender, IGameServer obj)
        {
            _clients.AddOrUpdate(obj.Id, sender, (x, y)=>sender);

            //TODO Needs to access the amin hub and message the user
            //    Saying that the server accepted our connection
            //    Issue is that, what if the user refreshes the page
            //    before we send the result message...maybe
            //    fuck it and make them rejoin if that happens? I dunno
            throw new NotImplementedException();
        }
    }
}