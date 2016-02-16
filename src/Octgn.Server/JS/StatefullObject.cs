using System;
using System.Collections.Generic;
using System.Dynamic;

namespace Octgn.Server.JS
{
    public class StatefullObject : DynamicObject
    {
        public string Name { get; protected set; }
        public string FullName
        {
            get
            {
                if (_parent == null) return Name;
                return _parent.Name + "." + Name;
            }
        }

        protected GameEngine Engine;
        protected dynamic dynamicObject;

        private StatefullObject _parent;
        private DynamicObject _object;
        public StatefullObject(string name, GameEngine engine, DynamicObject obj)
        {
            Name = name;
            Engine = engine;
            if (obj == null) obj = (DynamicObject)Engine.Javascript.ExecuteAndReturn("new Object()");
            _object = obj;
            dynamicObject = _object;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            var ret = _object.TrySetMember(binder, value);
            if (ret)
            {
                if (value is StatefullObject)
                {
                    (value as StatefullObject)._parent = this;
                    (value as StatefullObject).Name = binder.Name;
                }
                OnPropertyChanged(this, new PropertyChangedEventArgs(binder.Name, value));
            }
            return ret;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            return _object.TryGetMember(binder, out result);
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return _object.GetDynamicMemberNames();
        }

        protected virtual void OnPropertyChanged(StatefullObject sender, PropertyChangedEventArgs args)
        {
            if (_parent == null) return;
            _parent.OnPropertyChanged(sender, args);
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public static StatefullObject Create(GameEngine engine, string name, DynamicObject obj)
        {
            if (obj is StatefullObject)
                return (StatefullObject)obj;

            var ret = IsArray(obj)
                ? new StatefullArray(name, engine, obj)
                : new StatefullObject(name, engine, obj)
            ;

            return ret;
        }

        private static bool IsArray(DynamicObject obj)
        {
            dynamic d = obj;
            return (d.constructor != null && d.constructor.name == "Array");
        }
    }

}
