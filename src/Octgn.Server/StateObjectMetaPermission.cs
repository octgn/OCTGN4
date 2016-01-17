using System.Linq;

namespace Octgn.Server
{
    public struct StateObjectMetaPermission
    {
        private readonly string _value;
        public StateObjectMetaPermission(string value)
        {
            _value = value;
        }

        public StateObjectMetaPermission(string[] users)
        {
            _value = string.Join(":", users.OrderBy(x => x));
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
            if (obj == null)
                return false;
            if (_value == null)
                return false;
            if (obj is string)
                return obj.Equals(_value);
            if (obj is string[])
                return _value.Equals(string.Join(":", (obj as string[]).OrderBy(x => x)));
            if (obj is StateObjectMetaPermission)
                return _value.Equals(obj as string);
            return false;
        }
    }
}