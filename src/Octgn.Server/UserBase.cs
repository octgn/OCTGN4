using Castle.DynamicProxy;
using Octgn.Server.Networking;
using System.Linq;

namespace Octgn.Server
{
    public abstract class UserBase
    {
        public bool Connected { get; private set; }
        private GameServerSocket _socket;
        private IS2CComs RPC;
        private static ProxyGenerator _generator = new ProxyGenerator();
        public UserBase(GameServerSocket sock)
        {
            _socket = sock;
            Connected = true;
            RPC = _generator.CreateInterfaceProxyWithoutTarget<IS2CComs>(new ServerRpcInterceptor(sock));
        }

        internal void ProcessMessages()
        {
            var message = _socket.Read().FirstOrDefault();

            if(message == null)
            {
                Connected = false;
            }
        }
    }
    public class UnauthenticatedUser : UserBase
    {
        public UnauthenticatedUser(GameServerSocket sock): base (sock)
        {
        }
    }
}