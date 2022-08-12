
using BoardGameBot.Models;

namespace BoardGameBot.Database.Adapter.Repositories.Interfaces
{
	public interface IGameOwnerRepository
	{
		Task CreateGameOwnerAsync(GameOwner gameOwner);
		Task<GameOwner> GetGameOwnerAsync(long id);
		Task EditGameOwnerAsync(GameOwner gameOwner);
		Task<List<GameOwner>> GetAllGameOwnersAsync();
	}
}
