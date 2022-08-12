namespace BoardGameBot.Models
{
	public class Group
	{
		public Group(long id, string name, string description,
			ICollection<GameOwner>? members, ICollection<GameOwner>? admins, ICollection<Poll>? polls
		)
		{
			Id = id;
			Name = name;
			Description = description;
			Members = members;
			Admins = admins;
			Polls = polls;
		}

		public long Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public ICollection<GameOwner>? Members { get; set; }
		public ICollection<GameOwner>? Admins { get; set; }
		public ICollection<Poll>? Polls { get; set; }
	}
}
