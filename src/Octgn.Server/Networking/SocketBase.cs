using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;

namespace Octgn.Server.Networking
{
    public abstract class SocketBase : IDisposable
    {
        private TcpClient _socket;
        private CancellationTokenSource _cancelation;
        private NetworkProtocol _protocol;
        public SocketBase(TcpClient sock)
        {
            _socket = sock;
            _cancelation = new CancellationTokenSource();
        }

        public IEnumerable<NetworkProtocol.Packet> Read()
        {
            using (_socket)
            {
                var stream = _socket.GetStream();
                while (!_cancelation.IsCancellationRequested)
                {
                    var pack = _protocol.ReadPacket();

                    yield return pack;
                }
            }
        }

        public void Write(byte[] arr)
        {
            _socket.Client.Send(arr);
        }

        public void Dispose()
        {
            _cancelation.Cancel();
        }
    }
}
