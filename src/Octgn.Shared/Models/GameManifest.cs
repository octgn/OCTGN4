using Newtonsoft.Json;
using System;

namespace Octgn.Shared.Models
{
	public class GameManifest
	{
		public string Name { get; set; }
		public Version Version { get; set; }

		public GameManifest()
		{

		}

		public static GameManifest Parse(string str)
		{
			return JsonConvert.DeserializeObject<GameManifest>(str);
		}

		public override string ToString()
		{
			return JsonConvert.SerializeObject(this, Formatting.Indented);
		}
	}
}
