namespace Octgn.Server.Networking
{
    public interface IPacketInvoker
    {
        void Invoke(NetworkProtocol.Packet packet);
    }
}
