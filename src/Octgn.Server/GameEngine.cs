using Octgn.Server.JS;
using System;

namespace Octgn.Server
{
    internal class GameEngine : GameThread, IDisposable
    {
        internal GameResourceProvider Resources { get; private set; }
        internal StateHistory StateHistory {get;private set;}

        private JavascriptEngine _engine;
        internal OClass O { get; private set; }

        public UserList Users {get; private set;}

        public GameEngine(GameResourceProvider resources)
        {
            Resources = resources;
            Users = new UserList();
            StateHistory = new StateHistory();
            O = new OClass(this);
            _engine = new JavascriptEngine();
            _engine.AddObject("O", O);
            _engine.Execute(Resources.ReadEntryPoint());
            Start();
        }

        protected override void Run()
        {
            Users.ProcessUsers();
        }

        public override void Dispose()
        {
            _engine.Dispose();  
            base.Dispose();
        }
    }
}