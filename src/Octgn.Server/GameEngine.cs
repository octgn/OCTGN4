using System;
using System.Threading.Tasks;

namespace Octgn.Server
{
    public class GameEngine
    {
        public UserList Users { get; private set; }

        private Task _gameThread;

        public GameEngine()
        {
            Users = new UserList();
            _gameThread = new Task(Run, TaskCreationOptions.LongRunning);
        }

        private void Run()
        {
            while (true)
            {
                Users.ProcessUsers();
                System.Threading.Thread.Sleep(1);
            }
        }
    }
}