using Castle.DynamicProxy;
using System.Linq;

namespace Octgn.Shared.Networking
{
    public class RpcInterceptor : IInterceptor
    {
        private GameSocket _socket;
        public RpcInterceptor(GameSocket socket)
        {
            _socket = socket;
        }

        public void Intercept(IInvocation invocation)
        {
#pragma warning disable CC0008 // Use object initializer
            var packet = new NetworkProtocol.Packet();
            packet.Name = invocation.Method.Name;
            packet.Parameters = invocation.Method.GetParameters()
                .Select(x=>new NetworkProtocol.MethodParameter {
                    Name = x.Name,
                    Value = invocation.GetArgumentValue(x.Position)
                })
                .Where(x=>x.Value != null)
                .ToArray();
            _socket.Write(packet);
#pragma warning restore CC0008 // Use object initializer
        }
    }
}
