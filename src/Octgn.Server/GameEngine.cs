using System;

namespace Octgn.Server
{
    public class GameEngine : GameThread, IDisposable
    {
        public UserList Users
        {
            get;
            private set;
        }

        public GameEngine()
        {
            Users = new UserList();
        }

        protected override void Run()
        {
            Users.ProcessUsers();
        }
    }
}