using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Octgn.Shared;

namespace Octgn.Server
{
    internal abstract class GameThread : IDisposable
    {
        private static ILogger Log = LoggerFactory.Create<GameThread>();
        private Task _gameThread;
        private ConcurrentQueue<Task> _invokeQueue;
        private bool _disposing = false;
        public GameThread()
        {
            Log.Trace();
            _invokeQueue = new ConcurrentQueue<Task>();
            _gameThread = new Task(_run, TaskCreationOptions.LongRunning);
        }

        protected void Start()
        {
            _gameThread.Start();
        }

        internal Task Invoke(Action a)
        {
            var task = new Task(a);
            _invokeQueue.Enqueue(task);
            return task;
        }

        private void _run()
        {
            while (!_disposing)
            {
                Run();
                Task a = null;
                while (_invokeQueue.IsEmpty == false)
                {
                    if (_invokeQueue.TryDequeue(out a))
                    {
                        a.Start();
                        a.Wait();
                    }
                }

                Thread.Sleep(1);
            }
        }

        protected abstract void Run();

        public virtual void Dispose()
        {
            _disposing = true;
            _gameThread.Wait();
        }
    }
}