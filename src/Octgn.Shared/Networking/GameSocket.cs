using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Octgn.Shared.Networking
{
	public class GameSocket : IDisposable
	{
		private TcpClient _socket;
		private CancellationTokenSource _cancelation;
		private NetworkProtocol _protocol;
        private Task _backgroundReader;
        private ConcurrentQueue<NetworkProtocol.Packet> _packetQueue;
		public GameSocket(TcpClient sock)
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

        public GameSocket()
            :this(new TcpClient())
        {

        }

		public void Connect(IPEndPoint endpoint)
		{
			_socket.Connect(endpoint);
			_protocol = new NetworkProtocol(_socket);
            _backgroundReader = Task.Factory.StartNew(BackgroundReaderRun, _cancelation.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
		}

		public void Connect(string host)
		{
			var endpoint = ParseIPEndPoint(host);
			Connect(endpoint);
		}

		public IEnumerable<NetworkProtocol.Packet> Read()
		{
			while (!_cancelation.IsCancellationRequested)
			{
                NetworkProtocol.Packet pack = null;
                if (_packetQueue.TryDequeue(out pack))
                    yield return pack;
                else
                    yield return null;
			}
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

		public static int FreeTcpPort()
		{
			TcpListener l = new TcpListener(IPAddress.Loopback, 0);
			l.Start();
			int port = ((IPEndPoint)l.LocalEndpoint).Port;
			l.Stop();
			return port;
		}

		private static IPEndPoint ParseIPEndPoint(string text)
		{
			Uri uri;
			if (text.ToLower().StartsWith("localhost"))
				return new IPEndPoint(IPAddress.Loopback, int.Parse(text.Split(':')[1]));
			if (Uri.TryCreate(text, UriKind.Absolute, out uri))
				return new IPEndPoint(IPAddress.Parse(uri.Host), uri.Port < 0 ? 0 : uri.Port);
			if (Uri.TryCreate(String.Concat("tcp://", text), UriKind.Absolute, out uri))
				return new IPEndPoint(IPAddress.Parse(uri.Host), uri.Port < 0 ? 0 : uri.Port);
			if (Uri.TryCreate(String.Concat("tcp://", String.Concat("[", text, "]")), UriKind.Absolute, out uri))
				return new IPEndPoint(IPAddress.Parse(uri.Host), uri.Port < 0 ? 0 : uri.Port);
			throw new FormatException("Failed to parse text to IPEndPoint");
		}

		public void Dispose()
		{
			_cancelation.Cancel();
		}
	}
}
