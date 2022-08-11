namespace BoardGameBot.Models
{
	public class Group
	{
		public Group(int id, string name, string description,
			ICollection<GameOwner> allMembers, ICollection<GameOwner> allAdmins, ICollection<Poll> allPolls
		)
		{
			Id = id;
			Name = name;
			Description = description;
			AllMembers = allMembers;
			AllAdmins = allAdmins;
			AllPolls = allPolls;
		}

		public int Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public ICollection<GameOwner> AllMembers { get; set; }
		public ICollection<GameOwner> AllAdmins { get; set; }
		public ICollection<Poll> AllPolls { get; set; }
	}
}
