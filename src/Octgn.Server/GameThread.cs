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
        private ConcurrentQueue<Action> _invokeQueue;
        private bool _disposing = false;
        private int _threadId = 0;
        public GameThread()
        {
            Log.Trace();
            _invokeQueue = new ConcurrentQueue<Action>();
            _gameThread = new Task(_run, TaskCreationOptions.LongRunning);
        }

        protected void Start()
        {
            _gameThread.Start();
        }

        internal void Invoke(Action a)
        {
            if (Thread.CurrentThread.ManagedThreadId == _threadId)
                a();
            else {
                _invokeQueue.Enqueue(a);
            }
        }

        private void _run()
        {
            _threadId = Thread.CurrentThread.ManagedThreadId;
            while (!_disposing)
            {
                Run();
                Action a = null;
                while (_invokeQueue.IsEmpty == false)
                {
                    if (_invokeQueue.TryDequeue(out a))
                    {
                        a();
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