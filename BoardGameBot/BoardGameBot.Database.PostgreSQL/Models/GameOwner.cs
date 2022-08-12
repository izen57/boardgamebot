namespace BoardGameBot.Database.PostgreSQL.Models
{
	public class GameOwner
	{
		public GameOwner()
		{
			Games = new HashSet<Game>();
		}

		public long Id { get; set; }
		public string Name { get; set; }
		public string TGRef { get; set; }
		public Group GameOwnerGroup { get; set; }
		public virtual ICollection<Game> Games { get; set; }
	}
}
