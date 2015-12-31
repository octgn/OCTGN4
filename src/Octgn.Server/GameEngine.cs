using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Octgn.Server
{
    public class GameEngine : IDisposable
    {
        public UserList Users { get; private set; }

        private Task _gameThread;
        private ConcurrentQueue<Action> _invokeQueue;
        private System.Threading.CancellationTokenSource _cancelation;

        public GameEngine()
        {
            Users = new UserList();
            _invokeQueue = new ConcurrentQueue<Action>();
            _gameThread = new Task(Run, _cancelation.Token, TaskCreationOptions.LongRunning);
        }

        public void Invoke(Action a)
        {
            _invokeQueue.Enqueue(a);
        }

        private void Run()
        {
            while (!_cancelation.IsCancellationRequested)
            {
                Users.ProcessUsers();
                Action a = null;
                while(_invokeQueue.TryDequeue(out a))
                {
                    a();
                }
                System.Threading.Thread.Sleep(1);
            }
        }

        public void Dispose()
        {
            _cancelation.Cancel();
            _cancelation.Dispose();
        }
    }
}