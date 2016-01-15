using System.Collections.Generic;

namespace Octgn.Server
{
    public class UserList
    {
        private List<UserBase> _users;
        public UserList()
        {
            _users = new List<UserBase>();
        }

        public void AddUser(UserBase user)
        {
            lock(this)
                _users.Add(user);
        }

        public void ProcessUsers()
        {
            lock (this)
            {
                foreach(var user in _users.ToArray())
                {
                    if (user.Replaced)
                    {
                        _users.Remove(user);
                        continue;
                    }
                    user.ProcessMessages();
                }
            }
        }

        public void Broadcast(string name, object obj)
        {
            lock (this)
            {
                foreach(var user in _users)
                {
                    user.Send(name, obj);
                }
            }
        }
    }
}