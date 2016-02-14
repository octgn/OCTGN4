using Microsoft.ClearScript;
using Microsoft.ClearScript.V8;
using System;

namespace Octgn.Server.JS
{
    internal class JavascriptEngine : IDisposable
    {
        public dynamic Script { get; set; }
        private ScriptEngine _engine;
        public JavascriptEngine()
        {
            _engine = new V8ScriptEngine();
            Script = _engine.Script;
        }

        public void AddObject(string name, object o)
        {
            _engine.AddHostObject(name, o);
        }

        public void Execute(string js)
        {
            _engine.Execute(js);
        }

        public object ExecuteAndReturn(string js)
        {
            return _engine.Evaluate(js);
        }

        public void Dispose()
        {
            _engine.Dispose();
        }
    }
}
