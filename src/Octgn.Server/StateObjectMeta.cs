using System.Dynamic;
using System.Linq;

namespace Octgn.Server
{
    public class StateObjectMeta
    {
        public string Filter { get; set; }

        public StateObjectMetaPermission Read { get; set; }

        public StateObjectMetaPermission Write { get; set; }

        public StateObjectMeta(DynamicObject obj)
        {
            dynamic d = obj;
            if (obj.GetDynamicMemberNames().Any(x => x == "filter"))
                Filter = d.filter;
            if (obj.GetDynamicMemberNames().Any(x => x == "read"))
                Read = d.read;
            if (obj.GetDynamicMemberNames().Any(x => x == "write"))
                Write = d.write;
        }
    }
}
