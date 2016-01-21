using Octgn.Shared.Networking;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Octgn.Shared.Networking
{

	public class GameSocket : ISocket, IDisposable
	{
		public IPEndPoint Endpoint { get; private set; }
		private ISocket _socket;
		private Queue<NetworkProtocol.Packet> _queue;
		private bool _disposed;
		private Task _writeTask;

		//TODO need something continually trying to reconnect.
		public GameSocket(string host)
		{
			Endpoint = NetworkHelper.ParseIPEndPoint(host);
			_queue = new Queue<NetworkProtocol.Packet>();
		}

		public GameSocket(TcpClient existingConnection)
		{
			Endpoint = (IPEndPoint)existingConnection.Client.LocalEndPoint;
			_queue = new Queue<NetworkProtocol.Packet>();
			_socket = new RawSocket(existingConnection);
		}

		public void Write(NetworkProtocol.Packet packet)
		{
			lock (this)
			{
				if(_writeTask == null)
				{
					if (_socket == null || !TryWrite(packet))
					{
						_queue.Enqueue(packet);
						_writeTask = Task.Run(()=>WriteTaskRun());
					}
				}
				else
				{
					_queue.Enqueue(packet);
				}
			}
		}

		public NetworkProtocol.Packet Read()
		{
			lock (this)
			{
				NetworkProtocol.Packet ret = null;
				TryRead(out ret);
				return ret;
			}
		}

		public void Connect()
		{
			lock (this)
			{
				if(_socket == null)
				{
					_socket = new RawSocket();
					_socket.Connect(Endpoint);
				}
			}
		}

		public void Connect(IPEndPoint endpoint)
		{
			lock(this)
			{
				Endpoint = endpoint;
				if(_socket == null)
				{
					_socket = new RawSocket();
					_socket.Connect(endpoint);
				}
			}
		}

		private bool TryWrite(NetworkProtocol.Packet packet)
		{
			try
			{
				if (_socket == null) return false;
				_socket.Write(packet);
				return true;
			}
			catch
			{
			}
			return false;
		}

		private bool TryRead(out NetworkProtocol.Packet packet)
		{
			packet = null;
			try
			{
				if (_socket == null) return false;
				packet = _socket.Read();
				return true;
			}
			catch
			{
			}
			return false;
		}

		private void WriteTaskRun()
		{
			while (!_disposed)
			{
				NetworkProtocol.Packet pack = null;
				lock (this)
				{
					if(_queue.Count == 0)
					{
						_writeTask = null;
						return;
					}
					pack = _queue.Peek();
					if (TryWrite(pack))
					{
						_queue.Dequeue();
					}
				}
			}
		}

		public void Dispose()
		{
			_disposed = true;
			if(_socket != null)
			{
				_socket.Dispose();
			}
		}
	}
}