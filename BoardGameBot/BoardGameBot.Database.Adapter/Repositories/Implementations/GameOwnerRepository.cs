using BoardGameBot.Database.Adapter.Repositories.Interfaces;
using BoardGameBot.Models;

namespace BoardGameBot.Database.Adapter.Repositories.Implementations
{
	public class GameOwnerRepository: IGameOwnerRepository
	{
		public Task<long> CreateGameOwner(GameOwner gameOwner)
		{
			throw new NotImplementedException();
		}

		public Task<GameOwner> GetGameOwner(long id)
		{
			throw new NotImplementedException();
		}

		public Task<long> UpdateGameOwner(GameOwner gameOwner)
		{
			throw new NotImplementedException();
		}
	}
}
