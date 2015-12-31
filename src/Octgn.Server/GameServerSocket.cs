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
            //under some circumstances, it's not possible to detect
            //a client disconnecting if there's no data being sent
            //so it's a good idea to give them a timeout to ensure that 
            //we clean them up.
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
            //now we know that the amountTask is complete so
            //we can ask for its Result without blocking
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