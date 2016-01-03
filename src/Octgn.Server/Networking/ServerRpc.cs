using Castle.DynamicProxy;
using System.IO;

namespace Octgn.Server.Networking
{
    class ServerRpcInterceptor : IInterceptor
    {
        private GameServerSocket _socket;
        public ServerRpcInterceptor(GameServerSocket socket)
        {
            _socket = socket;
        }

        public void Intercept(IInvocation invocation)
        {
            using (var ms = new MemoryStream())
            using (var w = new BinaryWriter(ms))
            {
                w.Write(0x01);
                w.Write(invocation.Method.Name);
                var parameters = invocation.Method.GetParameters();
                for(var i = 0; i < parameters.Length; i++)
                {
                    w.Write(parameters[i].Name);
                    var val = invocation.GetArgumentValue(i);
                    if(val == null)
                    {
                        w.Write(0x02);
                        continue;
                    }
                    var ptype = parameters[i].ParameterType;
                    if (ptype.IsValueType 
                        || ptype == typeof(string)
                        || ptype == typeof(byte[])
                        || ptype == typeof(char[])
                    )
                    {
                        dynamic v = val;
                        w.Write(v);
                        continue;
                    }
                    w.Write(val.ToString());
                }
                w.Write(0x02);

                _socket.Write(ms.ToArray());
            }
        }
    }
}
