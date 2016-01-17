﻿using System;

namespace Octgn.Server.JS
{
    public class StateClass : StateObject
    {
        private GameEngine _engine;
        internal StateClass(string name, GameEngine engine) 
            : base(name)
        {
            _engine = engine;
        }

        protected override void OnPropertyChanged(StateObject sender, string name, object val)
        {
            _engine.Users.BroadcastRPC.StateChange(name, val);
        }
    }
}
