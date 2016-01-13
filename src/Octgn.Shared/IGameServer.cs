namespace Octgn.Shared
{
    public interface IGameServer
    {
        int Id { get; }

        int Port { get; }

        string Name { get; }
    }
}