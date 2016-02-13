using Octgn.Server.JS;
using System;

namespace Octgn.Server
{
    internal class GameEngine : GameThread, IDisposable
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
            O = new OClass(this);
            Javascript = new JavascriptEngine();
            Javascript.AddObject("O", O);
            Javascript.Execute("O.state.users = new Array()");
            if(Resources != null)
                Javascript.Execute(Resources.ReadEntryPoint());
            Start();
        }

        protected override void Run()
        {
            Users.ProcessUsers();
        }

        public override void Dispose()
        {
            Javascript.Dispose();  
            base.Dispose();
        }
    }
}