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
            com = new ComClass(engine);
            events = new EventsClass(engine);
            state = new StateClass("O.state", _engine);
        }
    }
}
