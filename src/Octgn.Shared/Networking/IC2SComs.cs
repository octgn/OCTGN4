namespace Octgn.Shared.Networking
{
    public interface IC2SComs
    {
        void Hello(string username);
    }

    public interface IS2CComs
    {
        void HelloResp(int id);
        void Kicked(string message);
    }
}