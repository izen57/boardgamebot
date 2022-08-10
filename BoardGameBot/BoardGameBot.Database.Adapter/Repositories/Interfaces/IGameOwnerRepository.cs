
using BoardGameBot.Models;

namespace BoardGameBot.Database.Adapter.Repositories.Interfaces
{
	public interface IGameOwnerRepository
	{
		public Task CreateGameOwner(GameOwner gameOwner);
		public Task<GameOwner> GetGameOwner(long id);
		public Task EditGameOwner(GameOwner gameOwner);
		public Task<List<GameOwner>> GetAllGameOwner();
	}
}
