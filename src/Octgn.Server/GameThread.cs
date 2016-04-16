using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Octgn.Shared;
using System.Diagnostics;

namespace Octgn.Server
{
    public abstract class GameThread : IDisposable
    {
        public static bool IgnoreThreadRestrictions;
        private static ILogger Log = LoggerFactory.Create<GameThread>();
        private Task _gameThread;
        private ConcurrentQueue<Action> _invokeQueue;
        private bool _disposing = false;
        private int _threadId = 0;
        protected GameThread()
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
            System.Diagnostics.Contracts.Contract.Requires(a != null);

            if (Thread.CurrentThread.ManagedThreadId == _threadId)
                a?.Invoke();
            else {
                _invokeQueue.Enqueue(a);
            }
        }

        internal void AssertRunningOnThisThread()
        {
            Debug.Assert(IgnoreThreadRestrictions || Thread.CurrentThread.ManagedThreadId == this._threadId, "This code must be running on the GameThread");
        }

        private void _run()
        {
            _threadId = Thread.CurrentThread.ManagedThreadId;
            Thread.CurrentThread.Name = "ServerGameThread:" + Thread.CurrentThread.ManagedThreadId;
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