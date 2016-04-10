using Octgn.Server.JS;
using System;

namespace Octgn.Server
{
    public class GameEngine : GameThread, IDisposable
    {
        internal GameResourceProvider Resources { get; private set; }
        internal StateHistory StateHistory {get;private set;}

        internal JavascriptEngine Javascript { get; private set; }
        internal OClass O { get; private set; }

        public UserList Users {get; private set;}

        public GameEngine(GameResourceProvider resources)
        {
            Resources = resources;
            Users = new UserList();
            StateHistory = new StateHistory();
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