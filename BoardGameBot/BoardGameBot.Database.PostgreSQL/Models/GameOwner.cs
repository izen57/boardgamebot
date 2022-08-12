namespace BoardGameBot.Database.PostgreSQL.Models
{
	public class GameOwner
	{
		public GameOwner()
		{
			Games = new HashSet<Game>();
		}

		public int Id { get; set; }
		public string Name { get; set; }
		public int GroupAdminId { get; set; }
		public int GroupMemberId { get; set; }
		public string TGRef { get; set; }
		public Group GroupAdmin { get; set; }
		public Group GroupMember { get; set; }
		public virtual ICollection<Game> Games { get; set; }
	}
}
