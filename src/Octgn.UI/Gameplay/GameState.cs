using Newtonsoft.Json;
using Octgn.Shared;
using System.Collections.Generic;

namespace Octgn.UI.Gameplay
{
    public class GameState
    {
        private string _fullState;
        private Dictionary<string, object> _state;
        private GameClient _client;

        public GameState(GameClient client)
        {
            _client = client;
            _fullState = "{}";
            _state = new Dictionary<string, object>();
        }

        internal void UpdateState(ObjectDiff diff)
        {
            lock (this)
            {
                var realo = val is string
                    ? JsonConvert.DeserializeObject(val as string) : val;

                if (!_state.ContainsKey(name))
                    _state.Add(name, realo);
                else
                    _state[name] = realo;

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