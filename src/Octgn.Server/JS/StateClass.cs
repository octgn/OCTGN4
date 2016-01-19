using System;

namespace Octgn.Server.JS
{
    public class StateClass : StateObject
    {
        private GameEngine _engine;
        internal StateClass(string name, GameEngine engine) 
            : base(name)
        {
            _engine = engine;
			// Here just an example of how and where to do something like this
            //AddProperty("stage", _engine.Resources.Manifest.InitialStage, false);
        }

        protected override void OnPropertyChanged(StateObject sender, string name, object val)
        {
            var id = _engine.StateHistory.StoreChange(name, val);
            if(id % 10 == 0)
                _engine.StateHistory.StoreFullState(id, this);
            _engine.Users.BroadcastRPC.StateChange(id, name, val);
        }
    }
}
