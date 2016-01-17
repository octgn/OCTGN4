using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Octgn.Shared;

namespace Octgn.Server
{
    public abstract class GameThread : IDisposable
    {
        private static ILogger Log = LoggerFactory.Create<GameThread>();
        private Task _gameThread;
        private ConcurrentQueue<Action> _invokeQueue;
        private bool _disposing = false;
        public GameThread()
        {
            Log.Trace();
            _invokeQueue = new ConcurrentQueue<Action>();
            _gameThread = new Task(_run, TaskCreationOptions.LongRunning);
			_gameThread.Start();
        }

        public void Invoke(Action a)
        {
            _invokeQueue.Enqueue(a);
        }

        private void _run()
        {
            while (!_disposing)
            {
                Run();
                Action a = null;
                while (_invokeQueue.IsEmpty == false)
                {
                    if (_invokeQueue.TryDequeue(out a))
                        a();
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