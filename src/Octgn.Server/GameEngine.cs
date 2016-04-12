using Octgn.Server.JS;
using System;
using System.Collections.Generic;

namespace Octgn.Server
{
    public class GameEngine : GameThread, IDisposable
    {
        internal GameResourceProvider Resources { get; private set; }

        internal JavascriptEngine Javascript { get; private set; }
        internal OClass O { get; private set; }

        public UserList Users {get; private set;}

        private List<ChangeTracker> _changeTrackers;

        public GameEngine(GameResourceProvider resources)
        {
            Resources = resources;
            _changeTrackers = new List<ChangeTracker>();
            Users = new UserList();
            Javascript = new JavascriptEngine();
			Javascript.Script.console = new
			{
				log = new Action<object>(ScriptLog)
			};
			O = new OClass(this);
            Javascript.AddObject("O", O);
			O.Init();
            if(Resources != null)
                Javascript.Execute(Resources.ReadEntryPoint());
            Start();
        }

        protected override void Run()
        {
            Users.ProcessUsers();
        }

        internal ChangeTracker CreateChangeTracker(object o)
        {
            var ret = new ChangeTracker(o);

            _changeTrackers.Add(ret);

            return ret;
        }

		private void ScriptLog(object msg)
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