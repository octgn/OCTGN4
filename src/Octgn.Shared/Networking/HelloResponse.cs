namespace Octgn.Shared.Networking
{
    public class HelloResponse
    {
        public int ServerId { get; set; }
        public int ServerPort { get; set; }
        public string ServerName { get; set; }
        public int UserId { get; set; }

        public HelloResponse()
        {

        }

        public HelloResponse(IGameServer server, int userId)
        {
            ServerId = server.Id;
            ServerPort = server.Port;
            ServerName = server.Name;
            UserId = userId;
        }
    }
}
