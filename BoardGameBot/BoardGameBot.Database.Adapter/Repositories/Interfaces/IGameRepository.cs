using BoardGameBot.Models;

namespace BoardGameBot.Database.Adapter.Repositories.Interfaces
{
	public interface IGameRepository
	{
		public Task<long> CreateGame(Game game);
		public Task<Game> GetGame(long id);
		public Task<long> UpdateGame(Game game);
	}
}
