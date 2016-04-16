using Castle.DynamicProxy;
using Octgn.Shared.Networking;
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
        public string Username { get; protected set; }
        internal UserBase Replaced { get; private set; }
        internal IS2CComs RPC { get; private set; }
        internal IInterceptor RPCInterceptor { get; private set; }
        protected GameServer Server;
        private GameSocket _socket;
        protected UserBase(GameServer server, GameSocket sock)
        {
            Username = "";
            Server = server;
            _socket = sock;
            Connected = true;
            RPCInterceptor = new RpcInterceptor(_socket);
            RPC = _generator.CreateInterfaceProxyWithoutTarget<IS2CComs>(RPCInterceptor);
            Id = System.Threading.Interlocked.Increment(ref _lastId);
        }

        protected UserBase(UserBase user)
        {
            _socket = user._socket;
            Server = user.Server;
            Connected = user.Connected;
            RPC = user.RPC;
            Id = user.Id;
            user.Replaced = this;
			RPCInterceptor = user.RPCInterceptor;
            Username = user.Username.Clone() as string;
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

        internal bool ProcessMessages()
        {
            this.Server.Engine.AssertRunningOnThisThread();
            var message = _socket.Read();

            if(message == null)
                return false;

            message.Invoke<IC2SComs>(this);
            return true;
        }

        protected void ReplaceSelf(UserBase user)
        {
			// Need to use this method so that things don't get GC'd
            Log.Debug("User {0}:{1} became {2}", user.Id, user.Username, user.GetType().Name);
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
            this.Server.Engine.AssertRunningOnThisThread();
            this.Username = username;
            dynamic context = new ExpandoObject();
            context.id = this.Id;
            context.username = this.Username;
            context.allow = true;
            this.Server.Engine.O.events.Fire_User_Authenticate(context);

            if(context.allow == false)
            {
                this.RPC.Kicked("You cannot join");
                return;
            }
            var resp = new HelloResponse(Server, Id);
            this.RPC.HelloResp(resp);

            var uc = this.Server.Engine.O.state.AddUser(this.Id, username);
            context = new ExpandoObject();
            context.user = uc;
            this.Server.Engine.O.events.Fire_User_Initialize(context);
			this.ReplaceSelf(new AuthenticatedUser(this));
        }
    }

    internal class AuthenticatedUser : UserBase
    {
        public AuthenticatedUser(UnauthenticatedUser user) 
            : base(user)
        {
        }

        public override void RemoteCall(string name, object obj)
        {
            this.Server.Engine.AssertRunningOnThisThread();
            this.Server.Engine.O.com.Fire_on(name, obj);
        }

		public override void GetResource(int reqId, string path)
		{
            this.Server.Engine.AssertRunningOnThisThread();
			var data = this.Server.Engine.Resources.Get(path);

			this.RPC.GetResourceResp(reqId, data.Data, data.ContentType);
		}
	}
}