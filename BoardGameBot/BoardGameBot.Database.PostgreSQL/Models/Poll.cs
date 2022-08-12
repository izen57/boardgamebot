namespace BoardGameBot.Database.PostgreSQL.Models
{
	public class Poll
	{
		public long Id { get; set; }
		public string Name { get; set; }
		public DateTime Time { get; set; }
		public int DayInterval { get; set; }
		public long? GroupId { get; set; }
		public Group? Group { get; set; }
	}
}
