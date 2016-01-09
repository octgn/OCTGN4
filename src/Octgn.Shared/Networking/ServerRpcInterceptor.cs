using Castle.DynamicProxy;
using System.IO;
using System.Linq;

namespace Octgn.Shared.Networking
{
    public class RpcInterceptor : IInterceptor
    {
        private SocketBase _socket;
        public RpcInterceptor(SocketBase socket)
        {
            _socket = socket;
        }

        public void Intercept(IInvocation invocation)
        {
            var packet = new NetworkProtocol.Packet();
            packet.Name = invocation.Method.Name;
            packet.Parameters = invocation.Method.GetParameters()
                .Select(x=>new NetworkProtocol.MethodParameter() {
                    Name = x.Name,
                    Value = invocation.GetArgumentValue(x.Position)
                })
                .Where(x=>x.Value != null)
                .ToArray();
            _socket.Write(packet);
        }
    }
}
