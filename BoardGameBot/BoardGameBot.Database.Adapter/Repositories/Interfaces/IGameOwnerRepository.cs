
using BoardGameBot.Models;

namespace BoardGameBot.Database.Adapter.Repositories.Interfaces
{
	public interface IGameOwnerRepository
	{
		Task CreateGameOwner(GameOwner gameOwner);
		Task<GameOwner> GetGameOwner(long id);
		Task EditGameOwner(GameOwner gameOwner);
		Task<List<GameOwner>> GetAllGameOwners();
	}
}
