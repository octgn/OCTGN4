using System.Collections.Generic;

namespace Octgn.Server.JS
{
    public class EventsClass
    {
        private GameEngine _engine;
        private Dictionary<string, List<dynamic>> _callbacks;
        internal EventsClass(GameEngine engine)
        {
            _engine = engine;
            _callbacks = new Dictionary<string, List<dynamic>>();
        }

        internal void Fire_User_Authenticate(EventContext ctx, object obj)
        {
            Fire_on("user.authenticate", ctx, obj);
        }

        internal void Fire_User_Initialize(EventContext ctx, object obj)
        {
            Fire_on("user.initialize", ctx, obj);
        }

        internal void Fire_on(string name, EventContext ctx, object obj)
        {
            if (!_callbacks.ContainsKey(name))
                return;
            foreach (var i in _callbacks[name])
            {
                i(ctx, obj);
            }
        }

        public void on(string name, dynamic callback)
        {
            if (!_callbacks.ContainsKey(name))
                _callbacks.Add(name, new List<dynamic>());
            _callbacks[name].Add(callback);
        }
    }
}