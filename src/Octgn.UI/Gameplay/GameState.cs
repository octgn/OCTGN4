using Octgn.Shared;
using System;
using System.Collections.Generic;

namespace Octgn.UI.Gameplay
{
    public class GameState
    {
        private object _fullState;
        private Dictionary<int, ObjectDiff> _state;
        private GameClient _client;

        public GameState(GameClient client)
        {
            _client = client;
            _fullState = new object();
            _state = new Dictionary<int, ObjectDiff>();
        }

        internal void UpdateState(ObjectDiff diff)
        {
            lock (this)
            {
                _state.Add(diff.Id, diff);
                _fullState = diff.Patch(_fullState);
                
                _client.UIRPC.FireStateUpdated(diff);
            }
        }

        internal void UpdateFullState(int id, string val)
        {
            throw new NotImplementedException();
            lock (this)
            {
                //_fullState = val;

                _client.UIRPC.FireStateReplaced(val);
            }
        }

        internal void SendStateToUI()
        {
            lock (this)
            {
                _client.UIRPC.FireStateReplaced(_fullState);
            }
        }
    }
}