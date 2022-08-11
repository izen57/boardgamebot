namespace BoardGameBot.Database.PostgreSQL.Models
{
	public class Group
	{
		public Group()
		{
			AllMembers = new HashSet<GameOwner>();
			AllAdmins = new HashSet<GameOwner>();
			AllPolls = new HashSet<Poll>();
		}

		public int Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public ICollection<GameOwner> AllMembers { get; set; }
		public ICollection<GameOwner> AllAdmins { get; set; }
		public ICollection<Poll> AllPolls { get; set; }
	}
}
