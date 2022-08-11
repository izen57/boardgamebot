namespace BoardGameBot.Models
{
	public class Poll
	{
		public Poll(int id, string name, DateTime timing, Group pollGroup)
		{
			Id = id;
			Name = name;
			Timing = timing;
			PollGroup = pollGroup;
		}

		public int Id { get; set; }
		public string Name { get; set; }
		public DateTime Timing { get; set; }
		public Group PollGroup { get; set; }
	}
}
