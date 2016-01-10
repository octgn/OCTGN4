using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Nancy.Security;

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

    public class User : IUserIdentity
    {
        public IEnumerable<string> Claims { get; } = new string[0];

        public string UserName { get; private set; }

        public string Sid { get; private set; }

        public User(string username, string sid)
        {
            UserName = username;
            Sid = sid;
        }
    }
}