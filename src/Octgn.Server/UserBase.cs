using System.Linq;

namespace Octgn.Server
{
    public abstract class UserBase
    {
        public bool Connected { get; private set; }
        private GameServerSocket _socket;
        public UserBase(GameServerSocket sock)
        {
            _socket = sock;
            Connected = true;
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