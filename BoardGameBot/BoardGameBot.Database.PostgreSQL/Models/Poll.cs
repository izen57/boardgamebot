namespace BoardGameBot.Database.PostgreSQL.Models
{
	public class Poll
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public DateTime Timing { get; set; }
		public Group PollGroup { get; set; }
	}
}
