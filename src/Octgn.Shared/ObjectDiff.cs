using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Dynamic;
using System.Linq;

namespace Octgn.Shared
{
    public class ObjectDiff
    {
        public Dictionary<string, object> Added { get; private set; } = new Dictionary<string, object>();
        public Dictionary<string, object> Modified { get; private set; } = new Dictionary<string, object>();
        public List<string> Deleted { get; private set; } = new List<string>();

        public ObjectDiff()
        {
            
        }

        public ObjectDiff(object prev, object cur)
        {
            Diff(prev, cur);
        }

        public void Diff(object prev, object cur, string parent = null)
        {
            Contract.Requires(prev != null);
            Contract.Requires(cur != null);

            var prefix = "";
            if (parent != null) prefix = parent + ".";

            var prevProps = EnumerateProperties(prev, parent).ToDictionary(x => x.Key, x => x.Value);
            var curProps = EnumerateProperties(cur, parent).ToDictionary(x => x.Key, x => x.Value);

            // Added and modified
            foreach(var prop in curProps)
            {
                object prevProp;
                if (!prevProps.TryGetValue(prop.Key, out prevProp))
                {
                    Added.Add(prefix + prop.Key, prop.Value);
                    continue;
                }

                if (IsValue(prop.Value))
                {
                    if (prop.Value == prevProp) continue;
                    Modified.Add(prefix + prop.Key, prop.Value);
                    continue;
                }

                // This is an object
                Diff(prevProp, prop.Value, prefix + prop.Key);
            }

            // Deleted
            foreach(var prevProp in prevProps.Where(x => !curProps.ContainsKey(x.Key)))
            {
                Deleted.Add(prefix + prevProp.Key);
            }
        }

        public static bool IsValue(object o)
        {
            if (o == null) return true;
            if (o is string) return true;
            if (o.GetType().IsValueType) return true;
            return false;
        }

        public static IEnumerable<KeyValuePair<string, object>> EnumerateProperties(object o, string parent = null)
        {
            Contract.Ensures(Contract.Result<IEnumerable<KeyValuePair<string, object>>>() == null);
            var prefix = "";
            if (parent != null) prefix = parent + ".";
            if (o is IEnumerable<KeyValuePair<string, object>>)
            {
                foreach(var i in (o as IEnumerable<KeyValuePair<string, object>>))
                {
                    yield return new KeyValuePair<string, object>(i.Key, i.Value);
                }
            }
            else if(o is DynamicObject)
            {
                var obj = o as DynamicObject;
                foreach(var name in Dynamitey.Dynamic.GetMemberNames(o))
                {
                    var kvp = new KeyValuePair<string, object>(name, Dynamitey.Dynamic.InvokeGet(o, name));
                    yield return kvp;
                }
            }
            else
            {
                foreach(var prop in o.GetType().GetProperties())
                {
                    var kvp = new KeyValuePair<string, object>(prop.Name, prop.GetValue(o));
                    yield return kvp;
                }
            }
        }
    }
}
