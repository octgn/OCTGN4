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
            foreach (var prop in curObj.Properties()) {
                JToken prevProp;
                var key = prop.Name;
                var prefix = parent == null ? string.Empty : parent;
                if (IsArray(cur)) {
                    key = "[" + key + "]";
                } else if (parent != null) {
                    prefix += ".";
                }
                if (!prevObj.TryGetValue(prop.Name, out prevProp)) {
                    Added.Add(prefix + key, prop.Value);
                    continue;
                }

                if (IsValue(prop.Value)) {
                    if (object.Equals(prop.Value, prevProp)) continue;
                    Modified.Add(prefix + key, prop.Value);
                    continue;
                }

                // This is an object
                Diff(prevProp, prop.Value, prefix + key);
            }

            // Deleted
            foreach (var prevProp in prevObj.Properties().Where(x => !curObj.Properties().Any(y => y.Name == x.Name))) {
                var key = prevProp.Name;
                var prefix = parent == null ? string.Empty : parent;
                if (IsArray(cur)) {
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

            var ret = ObjectToDictionary(patchObject);

            foreach (var a in Added) {
                var keys = a.Key.Split('.');
                var current = ret;
                for (var i = 0; i < keys.Length - 1; i++) {
                    current = (Dictionary<string, object>)current[keys[i]];
                }
                var val = IsValue(a.Value) ? a.Value : ObjectToDictionary(a.Value);
                current.Add(keys[keys.Length - 1], val);
            }

            foreach (var m in Modified) {
                var keys = m.Key.Split('.');
                var current = ret;
                for (var i = 0; i < keys.Length - 1; i++) {
                    current = (Dictionary<string, object>)current[keys[i]];
                }
                var val = IsValue(m.Value) ? m.Value : ObjectToDictionary(m.Value);
                current[keys[keys.Length - 1]] = val;
            }

            foreach (var d in Deleted) {
                var keys = d.Split('.');
                var current = ret;
                for (var i = 0; i < keys.Length - 1; i++) {
                    current = (Dictionary<string, object>)current[keys[i]];
                }
                current.Remove(keys[keys.Length - 1]);
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

        public static JObject ToJObject( object o ) {
            Contract.Ensures(Contract.Result<JObject>() != null);
            if (o == null) return JObject.FromObject(null);

            var ret = new JObject();
            foreach (var prop in EnumerateProperties(o)) {
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
    }
}
