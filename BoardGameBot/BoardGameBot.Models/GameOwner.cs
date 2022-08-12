namespace BoardGameBot.Models
{
	public class GameOwner
	{
		public GameOwner()
		{
		}

		public GameOwner(int id, string name, string tGRef, ICollection<Game> games)
		{
			Id = id;
			Name = name;
			TGRef = tGRef;
			Games = games;
		}

		public int Id { get; set; }
		public string Name { get; set; }
		public string TGRef { get; set; }
		public int GroupAdminId { get; set; }
		public int GroupMemberId { get; set; }
		public Group GroupAdmin { get; set; }
		public Group GroupMember { get; set; }
		public virtual ICollection<Game> Games { get; set; }
	}
}