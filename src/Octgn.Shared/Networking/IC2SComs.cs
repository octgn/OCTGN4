namespace Octgn.Shared.Networking
{
    public interface IC2SComs
    {
        void Hello(string username);
        void RemoteCall(string name, object obj);
    }

    public interface IS2CComs
    {
        void HelloResp(IGameServer server);
        void Kicked(string message);
        void RemoteCall(string name, object obj);
        void StateChange(string name, object val);
    }
}