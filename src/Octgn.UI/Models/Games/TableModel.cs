using Octgn.UI.Gameplay;

namespace Octgn.UI.Models.Games
{
    public class TableModel
    {
		public int Port { get; private set; }
        public int Id { get; private set; }

		public TableModel()
		{

		}

		public TableModel(GameClient client)
		{
			Id = client.Id;
			Port = client.Port;
		}
    }
}
