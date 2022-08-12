namespace BoardGameBot.Models
{
	public class GameOwner
	{
		public GameOwner()
		{
		}

		public GameOwner(long id, string name, long? groupAdminId, long? groupMemberId, string tGRef, Group? groupAdmin, Group? groupMember, ICollection<Game>? games)
		{
			Id = id;
			Name = name;
			GroupAdminId = groupAdminId;
			GroupMemberId = groupMemberId;
			TGRef = tGRef;
			GroupAdmin = groupAdmin;
			GroupMember = groupMember;
			Games = games;
		}

		public long Id { get; set; }
		public string Name { get; set; }
		public long? GroupAdminId { get; set; }
		public long? GroupMemberId { get; set; }
		public string TGRef { get; set; }
		public Group? GroupAdmin { get; set; }
		public Group? GroupMember { get; set; }
		public virtual ICollection<Game>? Games { get; set; }
	}
}