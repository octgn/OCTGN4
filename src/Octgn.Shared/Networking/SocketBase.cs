using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Octgn.Shared.Networking
{
	public abstract class SocketBase : IDisposable
	{
		private TcpClient _socket;
		private CancellationTokenSource _cancelation;
		private NetworkProtocol _protocol;
		public SocketBase(TcpClient sock)
		{
			_socket = sock;
			if (_socket.Connected)
				_protocol = new NetworkProtocol(_socket);
			_cancelation = new CancellationTokenSource();
		}

		public void Connect(IPEndPoint endpoint)
		{
			_socket.Connect(endpoint);
			_protocol = new NetworkProtocol(_socket);
		}

		public void Connect(string host)
		{
			var endpoint = ParseIPEndPoint(host);
			Connect(endpoint);
		}

		public IEnumerable<NetworkProtocol.Packet> Read()
		{
			var stream = _socket.GetStream();
			while (!_cancelation.IsCancellationRequested)
			{
				var pack = _protocol.ReadPacket();

				yield return pack;
			}
		}

		public void Write(NetworkProtocol.Packet packet)
		{
			_protocol.WritePacket(packet);
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
