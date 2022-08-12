namespace BoardGameBot.Models
{
	public class Game
	{
		public Game()
		{
		}

		public Game(
			long id, string title, string description, string players,
			string genre, int complexity, string letsPlay, string rules,
			int played, ICollection<GameOwner>? gameOwners
		)
		{
			Id = id;
			Title = title;
			Description = description;
			Players = players;
			Genre = genre;
			Complexity = complexity;
			LetsPlay = letsPlay;
			Rules = rules;
			Played = played;
			GameOwners = gameOwners;
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
