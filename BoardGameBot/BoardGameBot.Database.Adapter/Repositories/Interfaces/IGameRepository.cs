using BoardGameBot.Models;

namespace BoardGameBot.Database.Adapter.Repositories.Interfaces
{
	public interface IGameRepository
	{
		Task CreateGameAsync(Game game);
		Task<Game> GetGameAsync(long id);
		Task EditGameAsync(Game commonGame);
		Task<List<Game>> GetAllGamesAsync();
	}
}
