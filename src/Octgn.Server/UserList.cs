using Castle.DynamicProxy;
using Octgn.Shared.Networking;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Octgn.Server
{
    internal class UserList : IInterceptor
    {
        private static ProxyGenerator _generator = new ProxyGenerator();
        internal IS2CComs BroadcastRPC { get; private set; }
        private List<UserBase> _users;
        public UserList()
        {
            _users = new List<UserBase>();
            BroadcastRPC = _generator.CreateInterfaceProxyWithoutTarget<IS2CComs>(this);
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

        public void Intercept(IInvocation invocation)
        {
            lock (this)
            {
                foreach(var user in _users)
                {
                    user.RPCInterceptor.Intercept(invocation);
                }
            }
        }
    }
}