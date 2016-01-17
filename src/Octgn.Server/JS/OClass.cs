﻿using System;

namespace Octgn.Server.JS
{
    internal class OClass
    {
        public ComClass com { get; private set; }

        public StateClass state { get; private set; }

        private GameEngine _engine;

        public OClass(GameEngine engine)
        {
            _engine = engine;
            com = new ComClass(engine);
            state = new StateClass("O.state", _engine);
        }
    }
}
