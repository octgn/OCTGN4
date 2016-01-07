using Castle.DynamicProxy;
using System.IO;
using System.Linq;

namespace Octgn.Server.Networking
{
    class ServerRpcInterceptor : IInterceptor
    {
        private GameServerSocket _socket;
        public ServerRpcInterceptor(GameServerSocket socket)
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
