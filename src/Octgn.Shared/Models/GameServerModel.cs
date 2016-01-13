using System;

namespace Octgn.Shared.Models
{
	public class GameServerModel : IGameServer
	{
        public int Id { get; private set; }
        public int Port { get; private set; }
        public string Name { get; private set; }

		public GameServerModel()
		{

		}

		public GameServerModel(int id, string name, int port)
		{
			Id = id;
			Name = name;
			Port = port;
		}
	}
}
