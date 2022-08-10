using BoardGameBot.Models;

namespace BoardGameBot.Database.Adapter.Repositories.Interfaces
{
	public interface IGameRepository
	{
		public Task<long> CreateGame(Game game);
		public Task<Game> GetGame(long id);
		public Task<long> EditGame(Game commonGame);
		public Task<List<Game>> GetAllGames();
	}
}
