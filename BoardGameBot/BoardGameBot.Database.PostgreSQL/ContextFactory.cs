using Microsoft.EntityFrameworkCore;

namespace BoardGameBot.Database.PostgreSQL
{
	public class ContextFactory: IContextFactory
	{
		private readonly string _connectionString;
		public ContextFactory(string connectionString)
		{
			_connectionString = connectionString;
		}

		public BoardGameContext GetContext()
		{
			var optionBuilder = new DbContextOptionsBuilder<BoardGameContext>();
			optionBuilder.UseNpgsql(_connectionString);
			return new BoardGameContext(optionBuilder.Options);
		}
	}
}
