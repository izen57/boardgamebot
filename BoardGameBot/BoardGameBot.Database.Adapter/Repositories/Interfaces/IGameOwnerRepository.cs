
using BoardGameBot.Models;

namespace BoardGameBot.Database.Adapter.Repositories.Interfaces
{
	public interface IGameOwnerRepository
	{
		public Task<long> CreateGameOwner(GameOwner gameOwner);
		public Task<GameOwner> GetGameOwner(long id);
		public Task<long> UpdateGameOwner(GameOwner gameOwner);
	}
}
