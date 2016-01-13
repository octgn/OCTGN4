namespace Octgn.UI.Models.Games
{
    public class TableModel
    {
        public int Id { get; private set; }

		public TableModel()
		{

		}

		public TableModel(GameClient client)
		{
			Id = client.Id;
		}
    }
}
