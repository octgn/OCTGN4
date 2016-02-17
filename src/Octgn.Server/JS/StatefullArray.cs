using System;
using System.Collections.Generic;
using System.Dynamic;

namespace Octgn.Server.JS
{
    public class StatefullArray : StatefullObject
    {
        private List<dynamic> _items;

        public int length
        {
            get { return _items.Count; }
            set
            {
                if (value == _items.Count) return;
                while (value != _items.Count)
                {
                    if (value > _items.Count)
                        _items.Add(null);
                    else
                        _items.RemoveAt(_items.Count - 1);
                }
            }
        }

        public StatefullArray(string name, GameEngine engine, DynamicObject obj)
        : base(name, engine, obj)
        {
            if (obj != null)
            {
                if (engine.Javascript.Script.Array.isArray(obj))
                {
                    dynamic d = obj;
                    _items = new List<dynamic>(d.length);
                    for (var i = 0; i < d.length; i++)
                    {
                        var b = d[i];
                        _items.Add(d[i]);
                    }
                }
            }
            else
            {
                _items = new List<dynamic>();
            }
        }

        public int push(params dynamic[] args)
        {
            foreach (var o in args)
            {
                _items.Add(o);
            }
            return args.Length;
        }

        public dynamic pop()
        {
            if (_items.Count == 0) return null;
            var ret = _items[_items.Count - 1];
            _items.RemoveAt(_items.Count - 1);
            return ret;
        }

        public StatefullArray fill(dynamic o, int? start = null, int? end = null)
        {
            // What a stupid function. Seriously, who came up with
            //    the spec for this, a two year old?

            var s = start ?? 0;
            if (s < 0) s = _items.Count + s;
            s = Math.Max(0, s);
            if (s >= _items.Count) return this;

            var e = end ?? _items.Count - 1;
            if (e < 0) e = _items.Count + e;
            e = Math.Min(e, _items.Count - 1);
            if (e < 0) return this;

            for (var i = s; i <= e; i++)
            {
                _items[i] = o;
            }

            return this;
        }

        public StatefullArray reverse()
        {
            _items.Reverse();
            return this;
        }

        public dynamic shift()
        {
            if (_items.Count == 0) return null;
            var item = _items[0];
            _items.RemoveAt(0);
            return item;
        }

        public StatefullArray sort(Func<dynamic, dynamic, int> compare = null)
        {
            if (compare == null)
                compare = new Func<dynamic, dynamic, int>((a, b) =>
                {
                    var astring = a.ToString();
                    var bstring = b.ToString();
                    return string.Compare(astring, bstring);
                });

            _items.Sort(new Comparison<dynamic>(compare));

            return this;
        }

        public dynamic splice(int start, int deleteCount, params dynamic[] items)
        {
            dynamic ret = Engine.Javascript.ExecuteAndReturn("[]");

            start = start < 0
                ? Math.Max(0, _items.Count + start)
                : Math.Min(_items.Count - 1, start);

            if (deleteCount < 0) deleteCount = 0;
            var end = Math.Min(start + deleteCount, _items.Count - 1);
            if (start < deleteCount)
            {
                for (var i = start; i <= end; i++)
                {
                    ret.push(_items[start]);
                    _items.RemoveAt(start);
                }
            }

            for (var i = 0; i < items.Length; i++)
            {
                _items.Insert(i + start, items[i]);
            }

            return ret;
        }

        public int unshift(params dynamic[] args)
        {
            for (var i = args.Length - 1; i >= 0; i--)
            {
                _items.Insert(0, args[i]);
            }

            return args.Length;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            int poopoo = 0;
            if (!int.TryParse(binder.Name, out poopoo))
                return base.TrySetMember(binder, value);

            while (poopoo > (_items.Count - 1))
            {
                _items.Add(null);
				OnPropertyChanged(this, new PropertyChangedEventArgs(poopoo.ToString(), null));
            }

            _items[poopoo] = value;
			OnPropertyChanged(this, new PropertyChangedEventArgs(binder.Name, value));

            return true;
        }

		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
            int poopoo = 0;
            if (!int.TryParse(binder.Name, out poopoo))
                return base.TryGetMember(binder, out result);

			result = null;
			if (poopoo >= _items.Count) return true;

            result = _items[poopoo];

            return true;
		}

		public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
		{
			if((indexes[0] is int) == false)
				return base.TryGetIndex(binder, indexes, out result);
			var index = (int)indexes[0];

			result = null;
			if (index >= _items.Count) return true;

            result = _items[index];
			return true;
		}
	}
}