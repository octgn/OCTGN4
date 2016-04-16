using Castle.DynamicProxy;
using Octgn.Shared;
using Octgn.Shared.Networking;
using System;
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
        public string ConnectionStatus
        {
            get { return _connectionStatus; }
            set
            {
                if (value == _connectionStatus) return;
                _connectionStatus = value;
                UIRPC.ServerConnectionUpdated(_connectionStatus);
            }
        }

        public int Id { get; private set; }
        public int UserId { get; private set; }
        public GameUIRPC UIRPC { get; private set; }
        public ResourceResolver ResourceResolver { get; private set; }
        private GameSocket _socket;
        private GameState _state;
        private static int _nextId = 0;
        private string _connectionStatus;
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
            UIRPC = new GameUIRPC();
        }

        public bool Connect()
        {
            try
            {
                this.ConnectionStatus = "connecting";
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
                catch (Exception e)
                {
                    Log.Error(e.ToString());
                    throw;
                }
                finally
                {
                    if (!Thread.Yield())
                        Thread.Sleep(2);
                }
            }
            Log.Debug("Finished processing messages");
        }

        public void HelloResp(HelloResponse resp)
        {
            Log.Debug("HelloResp");
            ConnectionStatus = "connected";
            UserId = resp.UserId;
        }

        public void Kicked(string message)
        {
            Log.Standard("Kicked: " + message);
            Dispose();
        }

        public void RemoteCall(string name, object obj)
        {
            UIRPC.Invoke(name, obj);
        }

        public void StateChange(int id, ObjectDiff diff)
        {
            if (id != _lastState + 1)
            {
                //TODO should check to see if we missed a state change
                //    Cause we know they'll happen in order
                throw new NotImplementedException();
            }
            _lastState = id;
            _state.UpdateState(diff);
        }

        public void FullState(int id, string val)
        {
            _state.UpdateFullState(id, val);
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
            if (_readTask != null)
                _readTask.Wait();
            _socket.Dispose();
        }
    }
}
