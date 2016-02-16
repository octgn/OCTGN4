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
			Javascript.Script.__statefull = new Func<dynamic, dynamic>(x => {
				var ret = StatefullObject.Create(this, null, x);
				return ret;
			});
			O = new OClass(this);
            Javascript.AddObject("O", O);
			Javascript.Execute(@"
var statefull = function(obj){
	if(obj.constructor === Array){
		var real = obj.slice(0);
		var val = __statefull(obj);
		val.array = real;
	} else {
		var val = __statefull(obj);
	}
	return val;
};
");
			O.Init();
            //Javascript.Execute("O.state.users = statefull([])");
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