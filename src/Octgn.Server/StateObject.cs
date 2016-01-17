using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace Octgn.Server
{
    public class StateObject : DynamicObject, IDynamicMetaObjectProvider
    {
        public string Name { get; private set; }
        public StateObjectMeta Meta { get; set; }

        private DynamicObject _underlyingObject;
        private Dictionary<string, object> _properties;
        private StateObject _parent;
        protected StateObject(string name)
        {
            Name = name;
            _properties = new Dictionary<string, object>();
        }

        protected StateObject(string name, StateObject parent, DynamicObject o) : this(name)
        {
            _parent = parent;
            _underlyingObject = o;
            dynamic d = o;
            Type type = d.GetType();
            var meth = type.GetMethods().First(x => x.Name == "GetProperty" && x.GetParameters().Length == 2);
            foreach (var propName in o.GetDynamicMemberNames())
            {
                var val = meth.Invoke(d, new object[] { propName, new object[0] });
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
                            var so = new StateObject(propName, this, val as DynamicObject);
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
            if (value is DynamicObject)
            {
                if (binder.Name == "_meta")
                {
                    this.Meta = new StateObjectMeta(value as DynamicObject);
                    return true;
                }
                else
                {
                    var jo = ((DynamicObject)value);
                    if (jo.GetDynamicMemberNames().Any(x => x == "_meta"))
                    {
                        value = new StateObject(binder.Name, this, jo);
                    }
                }
            }

            if (!_properties.ContainsKey(binder.Name))
            {
                _properties.Add(binder.Name, value);
            }
            else
            {
                _properties[binder.Name] = value;
            }
            OnPropertyChanged(binder.Name, value);

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

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return _properties.Keys;
        }

        protected virtual void OnPropertyChanged(string name, object val)
        {
            _parent.OnPropertyChanged(this.Name + "." + name, val);
        }
    }
}