using System;

namespace Octgn.Server.JS
{
    internal class OClass
    {
        public ComClass com { get; private set; }

        public StateClass state { get; private set; }

        public EventsClass events { get; private set; }

        private GameEngine _engine;

        public OClass(GameEngine engine)
        {
            _engine = engine;
        }

		internal void Init()
		{
            com = new ComClass(_engine);
            events = new EventsClass(_engine);
            state = new StateClass(_engine);
		}
    }
}
