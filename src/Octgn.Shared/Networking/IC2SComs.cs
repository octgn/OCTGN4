namespace Octgn.Shared.Networking
{
    public interface IC2SComs
    {
        void Hello(string username);
        void JsInvoke(string name, object obj);
    }

    public interface IS2CComs
    {
        void HelloResp(IGameServer server);
        void Kicked(string message);
        void JsInvoke(string name, object obj);
    }
}