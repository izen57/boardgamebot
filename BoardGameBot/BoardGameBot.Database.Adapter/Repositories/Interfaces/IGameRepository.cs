using BoardGameBot.Models;

namespace BoardGameBot.Database.Adapter.Repositories.Interfaces
{
	public interface IGameRepository
	{
		Task CreateGame(Game game);
		Task<Game> GetGame(long id);
		Task EditGame(Game commonGame);
		Task<List<Game>> GetAllGames();
	}
}
