﻿using Microsoft.ClearScript.V8;
using Octgn.Server.JS;
using System;

namespace Octgn.Server
{
    public class GameEngine : GameThread, IDisposable
    {
        private JavascriptEngine _engine;
        private GameResourceProvider _resources;
        internal OClass O { get; private set; }

        public UserList Users {get; private set;}

        public GameEngine(GameResourceProvider resources)
        {
            _resources = resources;
            Users = new UserList();
            O = new OClass(this);
            _engine = new JavascriptEngine();
            _engine.AddObject("O", O);
            _engine.Execute(_resources.ReadEntryPoint());
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