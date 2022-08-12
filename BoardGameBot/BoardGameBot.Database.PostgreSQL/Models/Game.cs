namespace BoardGameBot.Database.PostgreSQL.Models
{
	public class Game
	{
		public Game()
		{
			GameOwners = new HashSet<GameOwner>();
		}

		public long Id { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public string Players { get; set; }
		public string Genre { get; set; }
		public int Complexity { get; set; }
		public string LetsPlay { get; set; }
		public string Rules { get; set; }
		public int Played { get; set; }
		public virtual ICollection<GameOwner>? GameOwners { get; set; }
	}
}