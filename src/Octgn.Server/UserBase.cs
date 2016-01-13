﻿using Castle.DynamicProxy;
using Octgn.Shared.Networking;
using System.Linq;
using System;
using System.Collections.Generic;
using Octgn.Shared;
using System.Reflection;

namespace Octgn.Server
{
    public abstract class UserBase: IC2SComs
    {
        private static ILogger Log = LoggerFactory.Create<GameThread>();
        public bool Connected { get; private set; }
        public int Id { get; private set; }
        public bool Replaced { get; private set; }
        private GameServerSocket _socket;
        protected IS2CComs RPC;
        private static ProxyGenerator _generator = new ProxyGenerator();
        private static int _lastId = 0;
        static UserBase()
        {
        }
        public UserBase(GameServerSocket sock)
        {
            _socket = sock;
            Connected = true;
            RPC = _generator.CreateInterfaceProxyWithoutTarget<IS2CComs>(new RpcInterceptor(sock));
            Id = System.Threading.Interlocked.Increment(ref _lastId);
        }

        public UserBase(UserBase user)
        {
            _socket = user._socket;
            Connected = user.Connected;
            RPC = user.RPC;
            Id = user.Id;
            Replaced = true;
        }

        internal void ProcessMessages()
        {
            var message = _socket.Read().FirstOrDefault();

            if(message == null)
            {
                Connected = false;
                return;
            }

            message.Invoke<IC2SComs>(this);
        }

        protected void ReplaceSelf(UserBase user)
        {
            Log.Debug("User {0} became {1}", user.Id, user.GetType().Name);
        }

        public virtual void Hello(string username)
        {
            throw new NotImplementedException();
        }
    }
    public class UnauthenticatedUser : UserBase
    {
        private GameServerSocket _sock;
        public UnauthenticatedUser(GameServerSocket sock): base (sock)
        {
            _sock = sock;
        }

        public override void Hello(string username)
        {
            this.RPC.HelloResp(this._sock._server);
        }
    }

    public class AuthenticatedUser : UserBase
    {
        public AuthenticatedUser(UnauthenticatedUser user) : base(user)
        {

        }
    }
}