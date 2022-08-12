namespace BoardGameBot.Database.PostgreSQL.Models
{
	public class Poll
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public DateTime Time { get; set; }
		public int DayInterval { get; set; }
		public int GroupId { get; set; }
		public Group Group { get; set; }
	}
}
