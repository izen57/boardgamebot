namespace BoardGameBot.Models
{
	public class Poll
	{
		public Poll(int id, string name, DateTime time, int dayInterval, int groupId, Group group)
		{
			Id = id;
			Name = name;
			Time = time;
			DayInterval = dayInterval;
			GroupId = groupId;
			Group = group;
		}

		public int Id { get; set; }
		public string Name { get; set; }
		public DateTime Time { get; set; }
		public int DayInterval { get; set; }
		public int GroupId { get; set; }
		public Group Group { get; set; }
	}
}
