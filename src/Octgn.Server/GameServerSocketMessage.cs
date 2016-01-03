namespace Octgn.Server
{
    public class GameServerSocketMessage
    {
        public bool Success
        {
            get;
            private set;
        }

        public byte[] Message
        {
            get;
            private set;
        }

        public GameServerSocketMessage(byte[] message)
        {
            Message = message;
            Success = true;
        }
    }
}