using Microsoft.AspNet.SignalR;

namespace Octgn.Client
{
    public class MainHub : Hub
    {
        public static IHubContext Instance
        {
            get { return GlobalHost.ConnectionManager.GetHubContext<MainHub>(); }
        }

        private UIBackend _backend;

        public MainHub(UIBackend be)
        {
            _backend = be;
        }

        public void Send(string name, string message)
        {
            Clients.All.addMessage(name, message);
        }

        public void PingClient()
        {
            System.Console.Beep();
        }

        public string HostGame(string username)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username))
                    throw new HubException(Resources.Text.MainHub_HostGame_UsernameValidationError);
                return _backend.HostGame(username);
            }
            catch (System.Exception e)
            {
                if (e is HubException) throw;
                //TODO Log exception
                throw new HubException(Resources.Text.MainHub_HostGame_UnhandledError);
            }
        }
    }
}