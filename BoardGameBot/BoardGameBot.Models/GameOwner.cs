namespace BoardGameBot.Models
{
	public class GameOwner
	{
		public GameOwner() { }

		public GameOwner(long id, string name, string tGRef, ICollection<Game> games)
		{
			Id = id;
			Name = name;
			TGRef = tGRef;
			Games = games;
		}

		public long Id { get; set; }
		public string Name { get; set; }
		public string TGRef { get; set; }

		public virtual ICollection<Game> Games { get; set; }

	}
}