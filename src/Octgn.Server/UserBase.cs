﻿using Castle.DynamicProxy;
using Octgn.Shared.Networking;
using System.Linq;
using System;
using Octgn.Shared;

namespace Octgn.Server
{
    public abstract class UserBase: IC2SComs
    {
        private static ILogger Log = LoggerFactory.Create<GameThread>();
        public bool Connected { get; private set; }
        public int Id { get; private set; }
        public bool Replaced { get; private set; }
        protected IS2CComs RPC;
        private GameSocket _socket;
        protected GameServer Server;
        private static ProxyGenerator _generator = new ProxyGenerator();
        private static int _lastId = 0;
        static UserBase()
        {
        }
        public UserBase(GameServer server, GameSocket sock)
        {
            Server = server;
            _socket = sock;
            Connected = true;
            RPC = _generator.CreateInterfaceProxyWithoutTarget<IS2CComs>(new RpcInterceptor(sock));
            Id = System.Threading.Interlocked.Increment(ref _lastId);
        }

        public UserBase(UserBase user)
        {
            _socket = user._socket;
            Server = user.Server;
            Connected = user.Connected;
            RPC = user.RPC;
            Id = user.Id;
            Replaced = true;
        }

        internal void ProcessMessages()
        {
            var message = _socket.Read().FirstOrDefault();

            if(message == null)
                return;

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

        public void Send(string name, object obj)
        {
            this.RPC.JsInvoke(name, obj);
        }

        public virtual void JsInvoke(string name, object obj)
        {
            throw new NotImplementedException();
        }
    }
    public class UnauthenticatedUser : UserBase
    {
        private GameSocket _sock;
        public UnauthenticatedUser(GameServer server, GameSocket sock)
            : base (server, sock)
        {
            _sock = sock;
        }

        public override void Hello(string username)
        {
            this.RPC.HelloResp(this.Server);
        }
    }

    public class AuthenticatedUser : UserBase
    {
        public AuthenticatedUser(UnauthenticatedUser user) 
            : base(user)
        {

        }

        public override void JsInvoke(string name, object obj)
        {
            this.Server.Engine.InvokeJsFunction(name, obj);
        }
    }
}