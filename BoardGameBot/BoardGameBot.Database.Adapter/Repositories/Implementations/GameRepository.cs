using BoardGameBot.Database.Adapter.Repositories.Interfaces;
using BoardGameBot.Models;

namespace BoardGameBot.Database.Adapter.Repositories.Implementations
{
	public class GameRepository: IGameRepository
	{
		public Task<long> CreateGame(Game game)
		{
			throw new NotImplementedException();
		}

		public Task<Game> GetGame(long id)
		{
			throw new NotImplementedException();
		}

		public Task<long> UpdateGame(Game game)
		{
			throw new NotImplementedException();
		}
	}
}
