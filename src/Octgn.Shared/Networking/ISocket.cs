using System;
using System.Collections.Generic;
using System.Net;

namespace Octgn.Shared.Networking
{
	public interface ISocket : IDisposable
	{
		NetworkProtocol.Packet Read();
		void Write(NetworkProtocol.Packet packet);
		void Connect(IPEndPoint endpoint);
	}
}
