using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Octgn.Server
{
    public class StateObject : DynamicObject, IDynamicMetaObjectProvider
    {
        public StateObjectMeta Meta { get; set; }

        private DynamicObject _underlyingObject;

        private Dictionary<string, object> _properties;

        public StateObject()
        {
            _properties = new Dictionary<string, object>();
        }

        public StateObject(DynamicObject o) : this()
        {
            _underlyingObject = o;
            dynamic d = o;
            Type type = d.GetType();
            var meth = type.GetMethods().First(x => x.Name == "GetProperty" &&  x.GetParameters().Length == 2);
            foreach (var propName in o.GetDynamicMemberNames())
            {
                var val = meth.Invoke(d, new object[] { propName, new object[0]});
                if (val == null)
                {
                    _properties[propName] = null;
                    continue;
                }
                if (val is DynamicObject)
                {
                    if (propName == "_meta")
                    {
                        this.Meta = new StateObjectMeta();
                        if((val as DynamicObject).GetDynamicMemberNames().Any(x=>x == "filter"))
                        {
                            this.Meta.Filter = val.filter;
                        }
                        continue;
                    }
                    else {
                        if ((val as DynamicObject).GetDynamicMemberNames().Any(x => x == "_meta"))
                        {
                            var so = new StateObject(val as DynamicObject);
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
                    this.Meta = new StateObjectMeta()
                    {
                        Filter = ((dynamic)value).filter,
                        Read = ((dynamic)value).read,
                        Write = ((dynamic)value).write,
                    };
                    return true;
                }
                else {
                    var jo = ((DynamicObject)value);
                    if (jo.GetDynamicMemberNames().Any(x => x == "_meta"))
                    {
                        value = new StateObject(jo);
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
            return true;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if(!_properties.ContainsKey(binder.Name))
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
    }

    public class StateObjectMeta
    {
        public string Filter { get; set; }
        public StateObjectMetaPermission Read { get; set; }
        public StateObjectMetaPermission Write { get; set; }
    }

    public struct StateObjectMetaPermission
    {
        private readonly string _value;
        public StateObjectMetaPermission(string value)
        {
            _value = value;
        }

        public StateObjectMetaPermission(string[] users)
        {
            _value = string.Join(":", users.OrderBy(x=> x));
        }

        public static implicit operator StateObjectMetaPermission(string value)
        {
            return new StateObjectMetaPermission(value);
        }

        public static implicit operator StateObjectMetaPermission(string[] users)
        {
            return new StateObjectMetaPermission(users);
        }

        public override string ToString()
        {
            return _value;
        }

        public override int GetHashCode()
        {
            return _value == null ? 0 : _value.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (_value == null) return false;
            if (obj is string)
                return obj.Equals(_value);
            if(obj is string[])
                return _value.Equals(string.Join(":", (obj as string[]).OrderBy(x=> x)));
            if (obj is StateObjectMetaPermission)
                return _value.Equals(obj as string);
            return false;
        }
    }
}
