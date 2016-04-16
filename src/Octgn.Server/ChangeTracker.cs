using Octgn.Shared;
using System.Collections.Generic;

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
            _changes = new Dictionary<int, ObjectDiff>();
        }

        public ObjectDiff ProcessChanges()
        {
            var diff = new ObjectDiff();

            if (diff.Diff(_objectProps, _object))
            {
                diff.Id = ++_prevId;
                // Store those changes(patch)
                _changes.Add(diff.Id, diff);

                // Update the _objectProps object with new object
                _objectProps = ObjectDiff.ObjectToDictionary(_object);
            }

            return diff;
        }
    }
}
