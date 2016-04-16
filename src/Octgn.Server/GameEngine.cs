using Octgn.Server.JS;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Octgn.Server
{
    public class GameEngine : GameThread, IDisposable
    {
        internal GameResourceProvider Resources { get; private set; }
        internal JavascriptEngine Javascript { get; private set; }
        internal OClass O { get; private set; }
        internal WaitHandle EngineInitialized { get; private set; }
        public UserList Users {get; private set;}

        private List<ChangeTracker> _changeTrackers;

        public GameEngine(GameResourceProvider resources)
        {
            EngineInitialized = new ManualResetEvent(false);
            Resources = resources;
            _changeTrackers = new List<ChangeTracker>();
            Users = new UserList();
            Start();
            this.Invoke(InitializeJS);
        }

        private void InitializeJS()
        {
            this.AssertRunningOnThisThread();

            Javascript = new JavascriptEngine();
			Javascript.Script.console = new
			{
				log = new Action<object>(ScriptLog)
			};
			O = new OClass(this);
#pragma warning disable CC0021 // Use nameof
            Javascript.AddObject("O", O);
#pragma warning restore CC0021 // Use nameof
            O.Init();
            if(Resources != null)
                Javascript.Execute(Resources.ReadEntryPoint());
            ((ManualResetEvent)EngineInitialized).Set();
        }

        protected override void Run()
        {
            this.AssertRunningOnThisThread();
            if (Users.ProcessUsers()) {
                ProcessChangeTrackers();
            }
        }

        internal ChangeTracker CreateChangeTracker(object o)
        {
            this.AssertRunningOnThisThread();
            var ret = new ChangeTracker(o);

            _changeTrackers.Add(ret);

            return ret;
        }

        internal void ProcessChangeTrackers()
        {
            this.AssertRunningOnThisThread();
            foreach(var c in _changeTrackers) {
                var diff = c.ProcessChanges();
                if (!diff.IsDifferent) continue;
                
                this.Users.BroadcastRPC.StateChange(diff.Id, diff);
            }
        }

		private static void ScriptLog(object msg)
		{
			System.Console.WriteLine(msg);
		}

        public override void Dispose()
        {
            Javascript.Dispose();  
            base.Dispose();
        }
    }
}