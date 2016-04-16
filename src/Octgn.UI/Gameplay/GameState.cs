using Newtonsoft.Json;
using Octgn.Shared;
using System.Collections.Generic;

namespace Octgn.UI.Gameplay
{
    public class GameState
    {
        private string _fullState;
        private Dictionary<int, ObjectDiff> _state;
        private GameClient _client;

        public GameState(GameClient client)
        {
            _client = client;
            _fullState = "{}";
            _state = new Dictionary<int, ObjectDiff>();
        }

        internal void UpdateState(ObjectDiff diff)
        {
            lock (this)
            {
                _state.Add(diff.Id, diff);

                _client.UIRPC.FirePropertyChanged(name, realo);
            }
        }

        internal void UpdateFullState(int id, string val)
        {
            lock (this)
            {
                _fullState = val;

                _client.UIRPC.FireStateReplaced(val);
            }
        }

        internal void SendStateToUI()
        {
            lock (this)
            {
                _client.UIRPC.FireStateReplaced(_fullState);
                foreach(var dingdong in _state)
                {
                    _client.UIRPC.FirePropertyChanged(dingdong.Key, dingdong.Value);
                }
            }
        }
    }
}