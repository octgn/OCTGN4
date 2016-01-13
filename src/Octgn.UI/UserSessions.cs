using System;
using System.Collections.Concurrent;

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
            if (string.IsNullOrWhiteSpace(sid))
                return null;
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
}