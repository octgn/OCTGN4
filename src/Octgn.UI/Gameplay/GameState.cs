using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

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

        internal void UpdateState(int id, string name, object val)
        {
            lock (this)
            {
                var realo = val is string
                    ? JsonConvert.DeserializeObject(val as string) : val;

                if (!_state.ContainsKey(name))
                    _state.Add(name, realo);
                else
                    _state[name] = realo;

                _client.User.UIRPC.firePropertyChanged(name, realo);
            }
        }

        internal void UpdateFullState(int id, string val)
        {
            lock (this)
            {
                _fullState = val;

                _client.User.UIRPC.fireStateReplaced(val);
            }
        }

        internal void SendStateToUI()
        {
            lock (this)
            {
                _client.User.UIRPC.fireStateReplaced(_fullState);
                foreach(var dingdong in _state)
                {
                    _client.User.UIRPC.firePropertyChanged(dingdong.Key, dingdong.Value);
                }
            }
        }
    }
}