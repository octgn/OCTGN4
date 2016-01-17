using Castle.DynamicProxy;
using Octgn.Shared;
using Octgn.Shared.Networking;
using Octgn.Shared.Resources;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Octgn.UI
{
	public class GameClient : IS2CComs, IDisposable
	{
		protected ILogger Log = LoggerFactory.Create<GameClient>();
		private static ProxyGenerator _generator = new ProxyGenerator();

		public IC2SComs RPC { get; private set; }
		public bool Connected
		{
			get { return _connected; }
			set
			{
				if (value == _connected) return;
				_connected = value;
				_user.UIRPC.GameStatusUpdated(_connected);
			}
		}

		public int Id { get; private set; }
		private GameSocket _socket;
		private User _user;
		private static int _nextId = 0;
		private bool _connected;
        private string _host;
		private Task _readTask;
		private CancellationTokenSource _cancellation;

		public GameClient(string host, User user)
		{
			Id = Interlocked.Increment(ref _nextId);
            _host = host;
			_user = user;
			_cancellation = new CancellationTokenSource();
			_socket = new GameSocket();
			RPC = _generator.CreateInterfaceProxyWithoutTarget<IC2SComs>(new RpcInterceptor(_socket));
		}

		public void Connect()
		{
			try
			{
				Log.Debug("Connecting...");
				_socket.Connect(_host);
				Log.Debug("Connected...");
				this.RPC.Hello(_user.UserName);
				_readTask = Task.Factory.StartNew(ProcessMessages, _cancellation.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
			}
			catch (Exception e)
			{
				Log.Error(e.ToString());
				this._user.UIRPC.gameJoinError(Text.MainHub_HostGame_UnhandledError);
			}
		}

		internal void ProcessMessages()
		{
			Log.Debug("Processing Messages");
			while (_cancellation.IsCancellationRequested == false)
			{
				try
				{
					var message = _socket.Read().FirstOrDefault();

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
            this._user.UIRPC.invoke(name, obj);
        }

        public void StateChange(int id, string name, object val)
        {
            //TODO should check to see if we missed a state change
            //    Cause we know they'll happen in order
            throw new NotImplementedException();
        }

        public void FullState(int id, string val)
        {
            // Blow away current state and replace it with this
            throw new NotImplementedException();
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
