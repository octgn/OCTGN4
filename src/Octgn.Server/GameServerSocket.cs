using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Threading;

namespace Octgn.Server
{
    public class GameServerSocket : IDisposable
    {
        private TcpClient _socket;
        private CancellationTokenSource _cancelation;
        public GameServerSocket(TcpClient sock)
        {
            _socket = sock;
            _cancelation = new CancellationTokenSource();
        }

        public IEnumerable<GameServerSocketMessage> Read()
        {
            using (_socket)
            {
                var buf = new byte[4096];
                var stream = _socket.GetStream();
                while (!_cancelation.IsCancellationRequested)
                {
                    var data = SingleRead(buf, stream).Result;
                    if(data == null)
                    {
                        yield break;
                    }
                    var ret = new GameServerSocketMessage(data);
                    yield return ret;
                }
            }
        }

        private async Task<byte[]> SingleRead(byte[] buf, NetworkStream stream)
        {
            // http://stackoverflow.com/questions/12630827/using-net-4-5-async-feature-for-socket-programming#answer-12631467
            var timeoutTask = Task.Delay(TimeSpan.FromSeconds(15));
            var amountReadTask = stream.ReadAsync(buf, 0, buf.Length, _cancelation.Token);
            var completedTask = await Task.WhenAny(timeoutTask, amountReadTask)
                                          .ConfigureAwait(false);
            if (completedTask == timeoutTask)
            {
                //var msg = Encoding.ASCII.GetBytes("Client timed out");
                //await stream.WriteAsync(msg, 0, msg.Length);
                return null;
            }
            var amountRead = amountReadTask.Result;
            if (amountRead == 0) return null; //end of stream.
            return buf;
        }

        public void Dispose()
        {
            _cancelation.Cancel();
        }
    }

    public class GameServerSocketMessage
    {
        public bool Success { get; private set; }
        public byte[] Message { get; private set; }

        public GameServerSocketMessage(byte[] message)
        {
            Message = message;
            Success = true;
        }
    }
}