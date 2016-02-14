using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace Octgn.Server
{
    public class StateObject : DynamicObject
    {
        public string Name { get; private set; }
        public string FullName { get; private set; }
        public StateObjectMeta Meta { get; set; }
        public bool IsArray { get; protected set; }

        private DynamicObject _underlyingObject;
        private Dictionary<string, object> _properties;
        private StateObject _parent;
        private GameEngine _engine;
        internal StateObject(string name, GameEngine engine)
            :this(name, null, engine, null)
        {
        }

        protected StateObject(string name, StateObject parent)
            : this(name, parent, null, null)
        {
        }

        internal StateObject(string name, StateObject parent, GameEngine engine, DynamicObject o) 
        {
            Name = name;
            FullName = "";
            _parent = parent;
            if(_parent != null)
            {
                FullName = _parent.FullName + ".";
            }
            FullName += Name;
            _engine = engine == null ? parent._engine : engine;
            _underlyingObject = o;
            _properties = new Dictionary<string, object>();
            if (o == null) return;
            dynamic d = o;
            try
            {
                var otype = d.constructor.name;
                if (d.constructor.name == "Array")
                {
                    this.IsArray = true;
                }
            }
            catch
            {

            }
            Type type = d.GetType();
            var meth = type.GetMethods().First(x => x.Name == "GetProperty" && x.GetParameters().Length == 2);
            foreach (var propName in o.GetDynamicMemberNames())
            {
                var val = meth.Invoke(d, new object[] { propName, new object[0] });
                if(val == null)
                {
                    val = d[propName];
                }
                if (val == null)
                {
                    _properties[propName] = null;
                    continue;
                }

                if (val is DynamicObject)
                {
                    if (propName == "_meta")
                    {
                        this.Meta = new StateObjectMeta(val as DynamicObject);
                        continue;
                    }
                    else
                    {
                        if ((val as DynamicObject).GetDynamicMemberNames().Any(x => x == "_meta"))
                        {
                            var so = new StateObject(propName, this, null, val as DynamicObject);
                            _properties[propName] = so;
                            continue;
                        }
                    }
                }

                _properties[propName] = val;
            }
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            AddProperty(binder.Name, value);

            return true;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (!_properties.ContainsKey(binder.Name))
            {
                result = null;
                return false;
            }

            result = _properties[binder.Name];
            return true;
        }

        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            AddProperty(indexes[0].ToString(), value, true);
            return true;
        }

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            return _properties.TryGetValue(indexes[0].ToString(), out result);
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return _properties.Keys;
        }

        protected void AddProperty(string name, object value, bool firePropertyChanged = true)
        {
            if (value is DynamicObject && !(value is StateObject))
            {
                if (name == "_meta")
                {
                    this.Meta = new StateObjectMeta(value as DynamicObject);
                    return;
                }
                value = new StateObject(name, this, null, (DynamicObject)value);
            }

            if (!_properties.ContainsKey(name))
            {
                _properties.Add(name, value);
            }
            else
            {
                _properties[name] = value;
            }
            if (firePropertyChanged)
                OnPropertyChanged(this, name, value);
        }

        protected virtual void OnPropertyChanged(StateObject sender, string name, object val)
        {
            const string propString = "{0}.{1}";
            const string arrString = "{0}['{1}']"; // Maybe this shouldn't always be quotes, but for now it's just easier this way
            var str = string.Format(IsArray ? arrString : propString, this.Name, name);
            _parent.OnPropertyChanged(sender, str, val);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("{");
            var first = true;
            foreach (var prop in _properties)
            {
                if (first) first = false;
                else sb.Append(",");
                sb.AppendFormat("'{0}':'{1}'", prop.Key, prop.Value.ToString());
            }
            sb.Append("}");
            return sb.ToString();
        }
    }
}