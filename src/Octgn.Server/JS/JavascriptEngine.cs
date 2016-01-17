using Microsoft.ClearScript.V8;
using System;

namespace Octgn.Server.JS
{
    internal class JavascriptEngine : IDisposable
    {
        private V8ScriptEngine _engine;
        public JavascriptEngine()
        {
            _engine = new V8ScriptEngine();
        }

        public void AddObject(string name, object o)
        {
            _engine.AddHostObject("O", o);
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
