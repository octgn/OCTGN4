namespace Octgn.Shared.Networking
{
    public interface IC2SComs
    {
        void Hello(string username);
        void RemoteCall(string name, object obj);
		void GetResource(int reqId, string path);
    }

    public interface IS2CComs
    {
        void HelloResp(HelloResponse resp);
        void Kicked(string message);
        void RemoteCall(string name, object obj);
        void StateChange(int id, string name, object val);
        void FullState(int id, string val);
		void SetLayout(string layout);
		void GetResourceResp(int reqId, byte[] data, string contentType);
    }
}