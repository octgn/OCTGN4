using Castle.DynamicProxy;
using Octgn.Shared;
using Octgn.Shared.Networking;
using Octgn.Shared.Resources;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Octgn.UI.Gameplay
{
	public class GameClient : IS2CComs, IDisposable
	{
		protected ILogger Log = LoggerFactory.Create<GameClient>();
		private static ProxyGenerator _generator = new ProxyGenerator();

		public IC2SComs RPC { get; private set; }
		public User User { get; private set; }
		public int Port { get; private set; }
		public bool Connected
		{
			get { return _connected; }
			set
			{
				if (value == _connected) return;
				_connected = value;
				User.UIRPC.GameStatusUpdated(_connected);
			}
		}

		public int Id { get; private set; }
		public ResourceResolver ResourceResolver { get; private set; }
		private GameSocket _socket;
        private GameState _state;
		private static int _nextId = 0;
		private bool _connected;
        private string _host;
		private Task _readTask;
		private CancellationTokenSource _cancellation;
        private int _lastState;

		public GameClient(string host, User user)
		{
			Id = Interlocked.Increment(ref _nextId);
            _host = host;
			User = user;
			_cancellation = new CancellationTokenSource();
			_socket = new GameSocket(host);
			Port = _socket.Endpoint.Port;
            _state = new GameState(this);
			RPC = _generator.CreateInterfaceProxyWithoutTarget<IC2SComs>(new RpcInterceptor(_socket));
			ResourceResolver = new ResourceResolver(this);
		}

		public bool Connect()
		{
			try
			{
				Log.Debug("Connecting...");
				_socket.Connect();
				Log.Debug("Connected...");
				this.RPC.Hello(User.UserName);
				_readTask = Task.Factory.StartNew(ProcessMessages, _cancellation.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
                return true;
			}
			catch (Exception e)
			{
				Log.Error(e.ToString());
                return false;
			}
		}

		internal void ProcessMessages()
		{
			Log.Debug("Processing Messages");
			while (_cancellation.IsCancellationRequested == false)
			{
				try
				{
					var message = _socket.Read();

					if (message == null)
						continue;

					message.Invoke<IS2CComs>(this);
				}
				catch(Exception e)
				{
					Log.Error(e.ToString());
					throw;
				}
				finally
				{
                    if(!Thread.Yield())
					    Thread.Sleep(2);
				}
			}
			Log.Debug("Finished processing messages");
		}

		public void HelloResp(IGameServer server)
		{
			Log.Debug("HelloResp");
			Connected = true;
		}

		public void Kicked(string message)
		{
			throw new NotImplementedException();
		}

        public void RemoteCall(string name, object obj)
        {
            this.User.UIRPC.invoke(name, obj);
        }

        public void StateChange(int id, string name, object val)
        {
            if(id != _lastState + 1)
            {
                //TODO should check to see if we missed a state change
                //    Cause we know they'll happen in order
                throw new NotImplementedException();
            }
            _lastState = id;
            _state.UpdateState(id, name, val);
        }

        public void FullState(int id, string val)
        {
            _state.UpdateFullState(id, val);
        }

		public void SetLayout(string layout)
		{
			this.User.UIRPC.fireSetLayout(layout);
		}

		public void GetResourceResp(int reqId, byte[] data, string contentType)
		{
			this.ResourceResolver.FinishRequest(reqId, data, contentType);
		}

        public void SendStateToUI()
        {
            _state.SendStateToUI();
        }

		public void Dispose()
		{
			_cancellation.Cancel();
			if(_readTask != null)
				_readTask.Wait();
			_socket.Dispose();
		}
	}
}
