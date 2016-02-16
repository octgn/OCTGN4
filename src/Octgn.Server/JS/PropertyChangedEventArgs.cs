using System;

namespace Octgn.Server.JS
{
    public class PropertyChangedEventArgs : EventArgs
    {
        public string PropertyName { get; set; }
        public object Value { get; set; }

        public PropertyChangedEventArgs(string name, object val)
        {
            PropertyName = name;
            Value = val;
        }
    }
}