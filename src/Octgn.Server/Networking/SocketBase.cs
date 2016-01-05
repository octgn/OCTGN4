using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Octgn.Server.Networking
{
    public class SocketBase : IDisposable
    {
        private TcpClient _socket;
        private CancellationTokenSource _cancelation;
        private NetworkProtocol _protocol;
        private IPacketInvoker _invoker;
        public SocketBase(TcpClient sock, IPacketInvoker invoker)
        {
            _socket = sock;
            _invoker = invoker;
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
                    _invoker.Invoke(pack);

                    yield return pack;
                }
            }
        }

        public void Write(byte[] arr)
        {
            _socket.Client.Send(arr);
        }

        public bool InvokeNextPacket()
        {
            var next = Read().FirstOrDefault();
            if (next == null) return false;
            _invoker.Invoke(next);
            return true;
        }

        private async Task<byte[]> SingleRead(byte[] buf, NetworkStream stream)
        {
            // http://stackoverflow.com/questions/12630827/using-net-4-5-async-feature-for-socket-programming#answer-12631467
            var timeoutTask = Task.Delay(TimeSpan.FromSeconds(15));
            var amountReadTask = stream.ReadAsync(buf, 0, buf.Length, _cancelation.Token);
            var completedTask = await Task.WhenAny(timeoutTask, amountReadTask).ConfigureAwait(false);
            if (completedTask == timeoutTask)
            {
                //var msg = Encoding.ASCII.GetBytes("Client timed out");
                //await stream.WriteAsync(msg, 0, msg.Length);
                return null;
            }

            var amountRead = amountReadTask.Result;
            if (amountRead == 0)
                return null; //end of stream.
            return buf;
        }

        public void Dispose()
        {
            _cancelation.Cancel();
        }
    }
}
