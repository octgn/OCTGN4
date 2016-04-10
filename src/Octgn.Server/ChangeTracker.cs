using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octgn.Server
{
    internal class ChangeTracker
    {
        private Dictionary<string, object> _objectProps;
        private object _object;

        public ChangeTracker(object o)
        {
            _object = o;
            _objectProps = new Dictionary<string, object>();
        }

        public void ProcessChanges()
        {

        }

        public void Change(object o)
        {

        }
    }
}
