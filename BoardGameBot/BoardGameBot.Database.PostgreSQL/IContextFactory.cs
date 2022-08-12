namespace BoardGameBot.Database.PostgreSQL
{
	public interface IContextFactory
	{
		BoardGameContext GetContext();
	}
}
