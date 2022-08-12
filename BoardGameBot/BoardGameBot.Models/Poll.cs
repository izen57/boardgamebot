namespace BoardGameBot.Models
{
	public class Poll
	{
		public Poll(long id, string name, DateTime time, int dayInterval, long? groupId, Group? group)
		{
			Id = id;
			Name = name;
			Time = time;
			DayInterval = dayInterval;
			GroupId = groupId;
			Group = group;
		}

		public long Id { get; set; }
		public string Name { get; set; }
		public DateTime Time { get; set; }
		public int DayInterval { get; set; }
		public long? GroupId { get; set; }
		public Group? Group { get; set; }
	}
}
