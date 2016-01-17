using System.Collections.Generic;

namespace Octgn.Server
{
    internal class UserList
    {
        private List<UserBase> _users;
        public UserList()
        {
            _users = new List<UserBase>();
        }

        internal void AddUser(UserBase user)
        {
            lock(this)
                _users.Add(user);
        }

        internal void ProcessUsers()
        {
            lock (this)
            {
                foreach(var user in _users.ToArray())
                {
                    if (user.Replaced != null)
                    {
                        _users.Remove(user);
						_users.Add(user.Replaced);
                        continue;
                    }
                    user.ProcessMessages();
                }
            }
        }

        internal void Broadcast(string name, object obj)
        {
            lock (this)
            {
                foreach(var user in _users)
                {
                    user.RPC.RemoteCall(name, obj);
                }
            }
        }
    }
}