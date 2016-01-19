using System;

namespace Octgn.UI.Gameplay
{
	public class ResourceResolver
	{
		private GameClient _client;
		public ResourceResolver(GameClient client)
		{
			_client = client;
		}

		public ResourceResolverResult Get(string path)
		{
			//TODO We need to setup the RPC to return values to continue with this.
			throw new NotImplementedException();
			return new ResourceResolverResult();
		}

		public class ResourceResolverResult
		{
			public bool Exists { get; set; }
			public byte[] Data { get; set; }
			public string ContentType { get; set; }

			public ResourceResolverResult()
			{
				ContentType = "application/octet-stream";
			}
		}
	}
}