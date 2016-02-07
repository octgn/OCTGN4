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

        internal void Fire_User_Authenticate(object ctx)
        {
            Fire_on("user.authenticate", ctx);
        }

        internal void Fire_User_Initialize(object ctx)
        {
            Fire_on("user.initialize", ctx);
        }

        internal void Fire_on(string name, object obj)
        {
            if (!_callbacks.ContainsKey(name))
                return;
            foreach (var i in _callbacks[name])
            {
                i(obj);
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