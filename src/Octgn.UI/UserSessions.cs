using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Nancy.Security;
using System.Security.Principal;
using System.Security.Claims;

namespace Octgn.UI
{
    public class UserSessions
    {
        private ConcurrentDictionary<string, User> _users;

        public UserSessions()
        {
            _users = new ConcurrentDictionary<string, User>();
        }

        public User Get(string sid)
        {
            if (string.IsNullOrWhiteSpace(sid)) return null;
            User ret = null;

            _users.TryGetValue(sid, out ret);
            return ret;
        }

        public User Create(string username)
        {
            var user = new User(username, Guid.NewGuid().ToString().Replace("-", "").ToLower());
            _users.AddOrUpdate(user.Sid, user, (x, y) => user);
            return user;
        }
    }

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

        public void JoinGame(int id, int port)
        {
            var client = new GameClient(port);
            client.Connect();
            _clients.AddOrUpdate(id, client, (x, y) => client);
        }
    }
}