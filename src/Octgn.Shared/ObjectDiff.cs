using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Dynamic;
using System.Linq;

namespace Octgn.Shared
{
    public class ObjectDiff
    {
        public int Id { get; set; }
        public DateTime DateCreated { get; set; }
        public Dictionary<string, object> Added { get; private set; } = new Dictionary<string, object>();
        public Dictionary<string, object> Modified { get; private set; } = new Dictionary<string, object>();
        public List<string> Deleted { get; private set; } = new List<string>();

        public bool IsDifferent { get; private set; }

        public ObjectDiff() {
            DateCreated = DateTime.Now;
        }

        public ObjectDiff( object prev, object cur ) {
            DateCreated = DateTime.Now;
            Diff(prev, cur);
        }

        public bool Diff( object prev, object cur, string parent = null ) {
            Contract.Requires(prev != null);
            Contract.Requires(cur != null);

            var prevObj = ToJObject(prev);
            var curObj = ToJObject(cur);

            // Added and modified
            foreach (var curProp in EnumerateProperties(curObj)) {
                var prevProp = GetProperty(prevObj, curProp.Name);

                var key = curProp.Name;
                var prefix = parent == null ? string.Empty : parent;

                if (IsArray(prevObj)) key = "[" + key + "]"; // We have a parent that's an array, use brackets
                else if (parent != null) prefix += "."; // We have a parent that's not an array, use dot notation

                if(prevProp == null) // Property just created now
                {
                    Added.Add(prefix + key, curProp.Value);
                    continue;
                }

                // Property already exists
                if (IsValue(curProp.Value)) {
                    if (object.Equals(curProp.Value, prevProp.Value)) continue; // Values are the same, no change here
                    Modified.Add(prefix + key, curProp.Value);
                    continue;
                }

                // This is an object
                Diff(prevProp.Value, curProp.Value, prefix + key);
            }

            // Deleted
            foreach (var prevProp in EnumerateProperties(prevObj).Where(x => !EnumerateProperties(curObj).Any(y => y.Name == x.Name))) {
                var key = prevProp.Name;
                var prefix = parent == null ? string.Empty : parent;
                if (IsArray(prev)) {
                    key = "[" + key + "]";
                } else if (parent != null) {
                    prefix += ".";
                }
                Deleted.Add(prefix + key);
            }
            IsDifferent = Added.Count + Modified.Count + Deleted.Count > 0;
            return IsDifferent;
        }

        public object Patch( object patchObject ) {
            Contract.Requires(patchObject != null);

            var ret = ToJObject(patchObject);

            foreach (var a in Added) {
                var keys = a.Key.Split('.');
                JToken current = ret;
                for (var i = 0; i < keys.Length - 1; i++) {
                    current = current[keys[i]];
                }
                var val = IsValue(a.Value) ? a.Value : ToJObject(a.Value);
                if(current is JObject) {
                    ((JObject)current).Add(keys[keys.Length - 1], JValue.FromObject(val));
                } else if(current is JArray) {
                    var numString = (keys[keys.Length - 1]).Trim('[', ']');
                    ((JArray)current).Insert(int.Parse(numString), JValue.FromObject(val));
                }
            }

            foreach (var m in Modified) {
                var keys = m.Key.Split('.');
                JToken current = ret;
                for (var i = 0; i < keys.Length - 1; i++) {
                    current = current[keys[i]];
                }
                var val = IsValue(m.Value) ? m.Value : ToJObject(m.Value);
                current[keys[keys.Length - 1]] = JValue.FromObject(val);
            }

            foreach (var d in Deleted) {
                var keys = d.Split('.');
                JToken current = ret;
                for (var i = 0; i < keys.Length - 1; i++) {
                    current = current[keys[i]];
                }
                if (current is JObject) {
                    ((JObject)current).Remove(keys[keys.Length - 1]);
                } else if (current is JArray) {
                    var numString = (keys[keys.Length - 1]).Trim('[', ']');
                    ((JArray)current).RemoveAt(int.Parse(numString));
                }
            }

            return ret;
        }

        public static bool IsValue( object o ) {
            if (o == null) return true;
            if (o is string) return true;
            if (o is JToken && ((JToken)o).Type != JTokenType.Object && ((JToken)o).Type != JTokenType.Array) return true;
            if (o.GetType().IsValueType) return true;
            return false;
        }

        public static bool IsArray( object o ) {
            if (o == null) return false;
            if (o.GetType().IsArray) {
                return true;
            } else if (o is JArray) {
                return true;
            } else if (o.GetType().Name == "V8ScriptItem") {
                if (((dynamic)o).constructor.name == "Array") {
                    return true;
                }
            }
            return false;
        }

        public static JToken ToJObject( object o ) {
            Contract.Ensures(Contract.Result<JObject>() != null);
            if (o == null) return JObject.FromObject(null);

            if (IsArray(o))
            {
                var reta = new JArray();
                foreach (var prop in EnumerateProperties(o))
                {
                    reta.Insert(int.Parse(prop.Name), prop.Value);
                }
                return reta;
            }
            var ret = new JObject();
            foreach (var prop in EnumerateProperties(o))
            {
                ret.Add(prop.Name, prop.Value);
            }
            return ret;
        }

        public static IEnumerable<JProperty> EnumerateProperties( object o, string parent = null ) {
            Contract.Ensures(Contract.Result<IEnumerable<JProperty>>() != null);
            var prefix = "";
            if (parent != null) prefix = parent + ".";
            if (o is IEnumerable<KeyValuePair<string, object>>) {
                foreach (var i in (o as IEnumerable<KeyValuePair<string, object>>)) {
                    yield return new JProperty(i.Key, GetValue(i.Value));
                }
            } else if (o is DynamicObject) {
                var obj = o as DynamicObject;
                foreach (var name in obj.GetDynamicMemberNames()) {
                    yield return new JProperty(name, GetValue(Dynamitey.Dynamic.InvokeGet(o, name)));
                }
            } else if (o is JObject) {
                var obj = o as JObject;
                foreach (var prop in obj.Properties()) {
                    yield return new JProperty(prop.Name, GetValue(prop.Value));
                }
            } else if (o is JArray) {
                var obj = o as JArray;
                for (var i = 0; i < obj.Count; i++) {
                    yield return new JProperty(i.ToString(), GetValue(obj[i]));
                }
            } else {
                foreach (var prop in o.GetType().GetProperties()) {
                    yield return new JProperty(prop.Name, GetValue(prop.GetValue(o)));
                }
            }
        }

        public static object GetValue( object o ) {
            if (o == null) return null;
            if (IsValue(o)) return o;
            if (IsArray(o)) {
                var props = EnumerateProperties(o)
                    .ToDictionary(x => int.Parse(x.Name), x => x.Value)
                    .OrderBy(x => x.Key)
                    .ToDictionary(x => x.Key, x => x.Value);
                if (props.Count == 0) return new JArray();

                var jarr = new List<object>();
                for (var i = 0; i < props.Last().Key + 1; i++) {
                    JToken val = null;
                    props.TryGetValue(i, out val);
                    jarr.Add(val);
                }
                return new JArray(jarr);
            } else return ToJObject(o);
        }

        public static JProperty GetProperty( JToken jt, string name )
        {
            foreach(var prop in EnumerateProperties(jt))
            {
                if (prop.Name == name) return prop;
            }
            return null;
        }
    }
}
