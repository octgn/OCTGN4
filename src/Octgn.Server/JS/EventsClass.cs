using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            Fire_on("user.authenticate", ctx).Wait();
        }

        internal void Fire_User_Initialize(object ctx)
        {
            Fire_on("user.initialize", ctx).Wait();
        }

        internal Task Fire_on(string name, object obj)
        {
            return _engine.Invoke(() =>
            {
                if (!_callbacks.ContainsKey(name))
                    return;
                foreach (var i in _callbacks[name])
                {
                    i(obj);
                }
            });
        }

        public void on(string name, dynamic callback)
        {
            _engine.Invoke(() =>
            {
                if (!_callbacks.ContainsKey(name))
                    _callbacks.Add(name, new List<dynamic>());
                _callbacks[name].Add(callback);
            });
        }
    }
}