using System;
using System.Collections.Generic;
using System.Threading;

namespace Octgn.UI.Gameplay
{
    public class ResourceResolver
    {
        private GameClient _client;
        private Dictionary<int, ResourceResolverRequest> _blockers;
        public ResourceResolver(GameClient client)
        {
            _client = client;
            _blockers = new Dictionary<int, ResourceResolverRequest>();
        }

        public ResourceResolverResult Get(string path)
        {
            ResourceResolverRequest req;
            lock (this)
            {
                req = new ResourceResolverRequest(path);
                _blockers.Add(req.Id, req);
            }

            var result = req.Get(() => _client.RPC.GetResource(req.Id, path));

            return result;
        }

        public void FinishRequest(int id, byte[] data, string contentType)
        {
            ResourceResolverRequest req;
            lock (this)
            {
                if (!_blockers.ContainsKey(id))
                    return;
                req = _blockers[id];
                _blockers.Remove(id);
            }
            req.OnResultReceived(data, contentType);
        }

        public class ResourceResolverResult
        {
            public int StatusCode { get; set; }
            public byte[] Data { get; set; }
            public string ContentType { get; set; }

            public ResourceResolverResult()
            {
                StatusCode = 404;
                ContentType = "application/octet-stream";
            }

            public ResourceResolverResult(int code)
            {
                StatusCode = code;
            }

            public ResourceResolverResult(byte[] data, string contentType)
            {
                ContentType = "application/octet-stream";
                if (data == null || data.Length == 0)
                {
                    StatusCode = 404;
                    Data = data;
                }
                else
                {
                    Data = data;
                    ContentType = contentType;
                }
            }
        }

        private class ResourceResolverRequest
        {
            public int Id { get; set; }
            public string Path { get; set; }
            private ManualResetEvent _waitHandle { get; set; }
            private ResourceResolverResult _result;

            private static int _lastId;
            public ResourceResolverRequest(string path)
            {
                Id = Interlocked.Increment(ref _lastId);
                Path = path;
                _waitHandle = new ManualResetEvent(false);
            }

            public ResourceResolverResult Get(Action act)
            {
                act();
                if (!_waitHandle.WaitOne(10000))
                {
                    return new ResourceResolverResult(408);
                }
                return _result;
            }

            public void OnResultReceived(byte[] data, string contentType)
            {
                _result = new ResourceResolverResult(data, contentType);
                _waitHandle.Set();
            }
        }
    }
}