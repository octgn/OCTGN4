﻿using Newtonsoft.Json.Linq;
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

            // TODO Should convert all things to Json.net files before comparing and storing and stuff
            // that should fix most issues I think
            var prevProps = EnumerateProperties(prev, parent).ToDictionary(x => x.Key, x => x.Value);
            var curProps = EnumerateProperties(cur, parent).ToDictionary(x => x.Key, x => x.Value);

            // Added and modified
            foreach (var prop in curProps) {
                object prevProp;
                var key = prop.Key;
                var prefix = parent == null ? string.Empty : parent;
                if (IsArray(cur)) {
                    key = "[" + key + "]";
                } else if (parent != null) {
                    prefix += ".";
                }
                if (!prevProps.TryGetValue(prop.Key, out prevProp)) {
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
            foreach (var prevProp in prevProps.Where(x => !curProps.ContainsKey(x.Key))) {
                var key = prevProp.Key;
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

        public static IEnumerable<KeyValuePair<string, object>> EnumerateProperties( object o, string parent = null ) {
            Contract.Ensures(Contract.Result<IEnumerable<KeyValuePair<string, object>>>() == null);
            var prefix = "";
            if (parent != null) prefix = parent + ".";
            if (o is IEnumerable<KeyValuePair<string, object>>) {
                foreach (var i in (o as IEnumerable<KeyValuePair<string, object>>)) {
                    yield return new KeyValuePair<string, object>(i.Key, GetValue(i.Value));
                }
            } else if (o is DynamicObject) {
                var obj = o as DynamicObject;
                foreach (var name in obj.GetDynamicMemberNames()) {
                    var kvp = new KeyValuePair<string, object>(name, GetValue(Dynamitey.Dynamic.InvokeGet(o, name)));
                    yield return kvp;
                }
            } else if (o is JObject) {
                var obj = o as JObject;
                foreach (var prop in obj.Properties()) {
                    var kvp = new KeyValuePair<string, object>(prop.Name, GetValue(prop.Value));
                    yield return kvp;
                }
            } else if (o is JArray) {
                var obj = o as JArray;
                for (var i = 0; i < obj.Count; i++) {
                    var kvp = new KeyValuePair<string, object>(i.ToString(), GetValue(obj[i]));
                    yield return kvp;
                }
            } else {
                foreach (var prop in o.GetType().GetProperties()) {
                    var kvp = new KeyValuePair<string, object>(prop.Name, GetValue(prop.GetValue(o)));
                    yield return kvp;
                }
            }
        }

        public static Dictionary<string, object> ObjectToDictionary( object o ) {
            if (IsValue(o)) throw new InvalidOperationException();
            if (o == null) return new Dictionary<string, object>();

            var dick = EnumerateProperties(o).ToDictionary(x => x.Key, x => {
                if (IsValue(x.Value)) return GetValue(x.Value);
                return ObjectToDictionary(x.Value);
            });

            return dick;
        }

        public static object GetValue( object o ) {
            if (o == null) return null;
            if (!(o is JToken)) {
                if (IsArray(o)) {
                    var props = EnumerateProperties(o)
                        .ToDictionary(x => int.Parse(x.Key), x => x.Value)
                        .OrderBy(x => x.Key)
                        .ToDictionary(x => x.Key, x => x.Value);
                    if (props.Count == 0) return new JArray();

                    var jarr = new List<object>();
                    for (var i = 0; i < props.Last().Key + 1; i++) {
                        object val = null;
                        props.TryGetValue(i, out val);
                        jarr.Add(val);
                    }
                    return new JArray(jarr);
                } else return o;
            }
            var j = (JToken)o;

            switch (j.Type) {
                case JTokenType.Boolean:
                    return j.Value<bool>();
                case JTokenType.Date:
                    return j.Value<DateTime>();
                case JTokenType.Float:
                    return j.Value<float>();
                case JTokenType.Guid:
                    return j.Value<Guid>();
                case JTokenType.Integer:
                    return j.Value<long>();
                case JTokenType.Undefined:
                case JTokenType.None:
                case JTokenType.Null:
                    return null;
                case JTokenType.Raw:
                case JTokenType.String:
                    return j.Value<string>();
                case JTokenType.TimeSpan:
                    return j.Value<TimeSpan>();
                case JTokenType.Uri:
                    return j.Value<Uri>();
                case JTokenType.Object:
                    return ObjectToDictionary(j.Value<object>());
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
