using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Octgn.Shared.Networking
{
	internal class RawSocket : ISocket, IDisposable
	{
		public int Port { get; private set; }
		private TcpClient _socket;
		private CancellationTokenSource _cancelation;
		private NetworkProtocol _protocol;
        private Task _backgroundReader;
        private ConcurrentQueue<NetworkProtocol.Packet> _packetQueue;
		public RawSocket(TcpClient sock)
		{
			_socket = sock;
            _packetQueue = new ConcurrentQueue<NetworkProtocol.Packet>();
			_cancelation = new CancellationTokenSource();
            if (_socket.Connected)
            {
                _protocol = new NetworkProtocol(_socket);
                _backgroundReader = Task.Factory.StartNew(BackgroundReaderRun, _cancelation.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            }
		}

        public RawSocket()
            :this(new TcpClient())
        {

        }

		public void Connect(IPEndPoint endpoint)
		{
			Port = endpoint.Port;
			_socket.Connect(endpoint);
			_protocol = new NetworkProtocol(_socket);
            _backgroundReader = Task.Factory.StartNew(BackgroundReaderRun, _cancelation.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
		}

		public NetworkProtocol.Packet Read()
		{
			if (_cancelation.IsCancellationRequested) return null;
			NetworkProtocol.Packet pack = null;
			_packetQueue.TryDequeue(out pack);
			return pack;
		}

		public void Write(NetworkProtocol.Packet packet)
		{
			_protocol.WritePacket(packet);
		}

        private void BackgroundReaderRun()
        {
			Thread.CurrentThread.Name = "GameSocket " + Thread.CurrentThread.ManagedThreadId;
            while(this._cancelation.IsCancellationRequested == false)
            {
                if (!_socket.Connected)
                {
                    if (!Thread.Yield()) Thread.Sleep(2);
                    continue;
                }

                var packet = _protocol.ReadPacket();
                if(packet != null)
                {
                    _packetQueue.Enqueue(packet);
                }
            }
        }

		public void Dispose()
		{
			_cancelation.Cancel();
		}
	}
}
