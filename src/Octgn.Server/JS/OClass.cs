using System;

namespace Octgn.Server.JS
{
    internal class OClass
    {
        public ComClass com { get; private set; }

        public StateClass state { get; private set; }

        public EventsClass events { get; private set; }

        public Func<dynamic, dynamic> statefull { get; private set; }

        private GameEngine _engine;

        public OClass(GameEngine engine)
        {
            _engine = engine;
            com = new ComClass(engine);
            events = new EventsClass(engine);
            state = new StateClass(_engine);
            statefull = CreateStatefull;
        }

        private dynamic CreateStatefull(dynamic d)
        {
            var ret = StatefullObject.Create(_engine, null, d);
            return ret;
        }
    }
}
