using Microsoft.ClearScript.V8;
using System;
using System.Collections.Generic;

namespace Octgn.Server
{
    public class GameEngine : GameThread, IDisposable
    {
        private V8ScriptEngine _engine;
        private GameResourceProvider _resources;
        private JsMainClass _jsmain;

        public UserList Users {get; private set;}

        public GameEngine(GameResourceProvider resources)
        {
            _resources = resources;
            Users = new UserList();
            _jsmain = new JsMainClass(this);
            _engine = new V8ScriptEngine();
            _engine.AddHostObject("O", _jsmain);
            _engine.Execute(_resources.ReadEntryPoint());
        }

        internal void InvokeJsFunction(string name, object o)
        {
            _jsmain.com.FireOn(name, o);
        }

        protected override void Run()
        {
            Users.ProcessUsers();
        }
    }

    internal class JsMainClass
    {
        public JsCom com { get; set; }

        private GameEngine _engine;

        public JsMainClass(GameEngine engine)
        {
            _engine = engine;
            com = new JsCom(engine);
        }
    }

    public class JsCom
    {
        private GameEngine _engine;
        private Dictionary<string, List<dynamic>> _callbacks;
        internal JsCom(GameEngine engine)
        {
            _engine = engine;
			_callbacks = new Dictionary<string, List<dynamic>>();
        }

        internal void FireOn(string name, object obj)
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