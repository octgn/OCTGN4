using Castle.DynamicProxy;
using Octgn.Shared.Networking;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Octgn.Server
{
    public class UserList : IInterceptor
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

        internal UserBase Get(int id)
        {
            lock(this)
                return _users.FirstOrDefault(x => x.Id == id);
        }

        internal bool ProcessUsers()
        {
            // TODO this lock could be a potential deadlock situation, should overanalize it.
            lock (this)
            {
                var anyProcessed = false;
                foreach(var user in _users.ToArray())
                {
                    if (user.Replaced != null)
                    {
                        _users.Remove(user);
						_users.Add(user.Replaced);
                        continue;
                    }
                    if (user.ProcessMessages())
                        anyProcessed = true;
                }
                return anyProcessed;
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