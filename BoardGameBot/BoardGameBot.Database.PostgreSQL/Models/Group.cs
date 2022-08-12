namespace BoardGameBot.Database.PostgreSQL.Models
{
	public class Group
	{
		public Group()
		{
			Members = new HashSet<GameOwner>();
			Admins = new HashSet<GameOwner>();
			Polls = new HashSet<Poll>();
		}

		public int Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public ICollection<GameOwner> Members { get; set; }
		public ICollection<GameOwner> Admins { get; set; }
		public ICollection<Poll> Polls { get; set; }
	}
}
