using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Octgn.Shared
{
    public interface ILogger
    {
        void Setup(string type);
        void Trace(string message= "", [CallerMemberName] string caller = "", params object[] args);
        void Debug(string message, params object[] args);
        void Standard(string message, params object[] args);
        void Error(string message, params object[] args);
    }

    public class DeadLog : ILogger
    {
        public string TypeName { get; private set; }

        public void Setup(string type)
        {
            TypeName = type;
        }

        public void Debug(string message, params object[] args)
        {
        }

        public void Error(string message, params object[] args)
        {
        }

        public void Standard(string message, params object[] args)
        {
        }

        public void Trace(string message, [CallerMemberName] string caller = "", params object[] args)
        {
        }
    }

    public static class LoggerFactory
    {
        private static Type _logger;
        static LoggerFactory()
        {
            _logger = typeof(DeadLog);
       }
        public static void SetDefault<T>() where T : ILogger
        {
            _logger = typeof(T);
        }

        public static ILogger Create<T>()
        {
            return Create(typeof(T).Name);
        }

        public static ILogger Create(string typename)
        {
            var l = (ILogger)Activator.CreateInstance(_logger);
            l.Setup(typename);
            return l;
        }
    }
}
