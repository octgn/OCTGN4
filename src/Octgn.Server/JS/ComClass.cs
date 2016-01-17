using System.Collections.Generic;

namespace Octgn.Server.JS
{
    public class ComClass
    {
        private GameEngine _engine;
        private Dictionary<string, List<dynamic>> _callbacks;
        internal ComClass(GameEngine engine)
        {
            _engine = engine;
			_callbacks = new Dictionary<string, List<dynamic>>();
        }

        internal void Fire_on(string name, object obj)
        {
            _engine.Invoke(() => {
                if (!_callbacks.ContainsKey(name))
                    return;
                foreach(var i in _callbacks[name])
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

        public void broadcast(string name, object obj)
        {
            _engine.Invoke(() => {
                _engine.Users.Broadcast(name, obj);
            });
        }
    }
}
