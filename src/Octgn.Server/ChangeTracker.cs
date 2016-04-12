using Octgn.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octgn.Server
{
    internal class ChangeTracker
    {
        private Dictionary<int, ObjectDiff> _changes;
        private Dictionary<string, object> _objectProps;
        private object _object;
        private int _prevId;

        public ChangeTracker(object o)
        {
            _object = o;
            _objectProps = new Dictionary<string, object>();
        }

        public void ProcessChanges()
        {
            var diff = new ObjectDiff();

            diff.Diff(_objectProps, _object);

            // Store those changes(patch)
            _changes.Add(_prevId++, diff);

            // Update the _objectProps object with new object
            _objectProps = ObjectDiff.ObjectToDictionary(_object);
        }
    }
}
