using Castle.DynamicProxy;
using Octgn.Shared.Networking;
using System.Linq;
using System;
using Octgn.Shared;
using Octgn.Server.JS;
using System.Dynamic;

namespace Octgn.Server
{
    public abstract class UserBase: IC2SComs
    {
        private static ILogger Log = LoggerFactory.Create<UserBase>();
        private static ProxyGenerator _generator = new ProxyGenerator();
        private static int _lastId = 0;
        public bool Connected { get; private set; }
        public int Id { get; private set; }
        internal UserBase Replaced { get; private set; }
        internal IS2CComs RPC { get; private set; }
        internal IInterceptor RPCInterceptor { get; private set; }
        protected GameServer Server;
        private GameSocket _socket;
        public UserBase(GameServer server, GameSocket sock)
        {
            Server = server;
            _socket = sock;
            Connected = true;
            RPCInterceptor = new RpcInterceptor(_socket);
            RPC = _generator.CreateInterfaceProxyWithoutTarget<IS2CComs>(RPCInterceptor);
            Id = System.Threading.Interlocked.Increment(ref _lastId);
        }

        public UserBase(UserBase user)
        {
            _socket = user._socket;
            Server = user.Server;
            Connected = user.Connected;
            RPC = user.RPC;
            Id = user.Id;
            user.Replaced = this;
			RPCInterceptor = user.RPCInterceptor;
        }

        public virtual void Hello(string username)
        {
            throw new NotImplementedException();
        }

        public virtual void RemoteCall(string name, object obj)
        {
            throw new NotImplementedException();
        }

        public virtual void BrowserOpened()
        {
            throw new NotImplementedException();
        }

		public virtual void GetResource(int reqId, string path)
		{
			throw new NotImplementedException();
		}

        internal void ProcessMessages()
        {
            var message = _socket.Read();

            if(message == null)
                return;

            message.Invoke<IC2SComs>(this);
        }

        protected void ReplaceSelf(UserBase user)
        {
			// Need to use this method so that things don't get GC'd
            Log.Debug("User {0} became {1}", user.Id, user.GetType().Name);
        }
	}
    internal class UnauthenticatedUser : UserBase
    {
        private GameSocket _sock;
        public UnauthenticatedUser(GameServer server, GameSocket sock)
            : base (server, sock)
        {
            _sock = sock;
        }

        public override void Hello(string username)
        {
            dynamic context = new ExpandoObject();
            context.user = new UserClass(this);
            context.allow = true;
            this.Server.Engine.O.events.Fire_User_Authenticate(context);

            if(context.allow == false)
            {
                this.RPC.Kicked("You cannot join");
                return;
            }
            this.RPC.HelloResp(this.Server);

            foreach(var change in Server.Engine.StateHistory.GetLatestChanges())
            {
                if (change.IsFullState)
                {
                    this.RPC.FullState(change.Id, change.Change);
                }
                else
                {
                    this.RPC.StateChange(change.Id, change.Name, change.Change);
                }
            }
			this.ReplaceSelf(new AuthenticatedUser(this));
        }
    }

    internal class AuthenticatedUser : UserBase
    {
        public AuthenticatedUser(UnauthenticatedUser user) 
            : base(user)
        {
            dynamic context = new ExpandoObject();
            context.user = new UserClass(this);
            context.layout = "";
            this.Server.Engine.O.events.Fire_User_Initialize(context);

            this.RPC.SetLayout(context.layout);
        }

        public override void RemoteCall(string name, object obj)
        {
            this.Server.Engine.O.com.Fire_on(name, obj);
        }

		public override void GetResource(int reqId, string path)
		{
			var data = this.Server.Engine.Resources.Get(path);

			this.RPC.GetResourceResp(reqId, data.Data, data.ContentType);
		}
	}
}