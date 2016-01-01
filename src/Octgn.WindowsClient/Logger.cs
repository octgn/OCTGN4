using System;
using System.Runtime.CompilerServices;
using Octgn.Shared;

namespace Octgn.WindowsClient
{
    public class Logger : ILogger
    {
        private string _typeName;
        public void Setup(string type)
        {
            _typeName = type;
        }

        public void Debug(string message, params object[] args)
        {
            WriteLine(message, args: args);
        }

        public void Error(string message, params object[] args)
        {
            WriteLine(message, args: args);
        }

        public void Standard(string message, params object[] args)
        {
            WriteLine(message, args: args);
        }

        public void Trace(string message = "", [CallerMemberName] string caller = "", params object[] args)
        {
            WriteLine($"{caller}() | " + message, args: args);
        }

        protected void WriteLine(string str, [CallerMemberName] string cmem = "", params object[] args)
        {
            var strr = $"[{cmem.ToUpper()} {DateTime.Now}] - {string.Format(str, args)}"; 
            System.Diagnostics.Debug.WriteLine(strr);
        }
    }
}
