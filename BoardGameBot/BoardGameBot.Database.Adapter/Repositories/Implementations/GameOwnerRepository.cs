using AutoMapper;

using BoardGameBot.Database.Adapter.Repositories.Interfaces;
using BoardGameBot.Database.PostgreSQL;
using BoardGameBot.Database.PostgreSQL.Models;

using Microsoft.EntityFrameworkCore;

using CommonModels = BoardGameBot.Models;

namespace BoardGameBot.Database.Adapter.Repositories.Implementations
{
	public class GameOwnerRepository: IGameOwnerRepository
	{
		private readonly IContextFactory _contextFactory;
		private readonly IMapper _mapper;

		public GameOwnerRepository(IContextFactory contextFactory, IMapper mapper)
		{
			_contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
			_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
		}

		public async Task CreateGameOwnerAsync(CommonModels.GameOwner commonGameOwner)
		{
			using var database = _contextFactory.GetContext();

			var gameOwner = _mapper.Map<GameOwner>(commonGameOwner);
			await database.GameOwners.AddAsync(gameOwner);
			await database.SaveChangesAsync();
		}

		public async Task<CommonModels.GameOwner> GetGameOwnerAsync(long id)
		{
			using var database = _contextFactory.GetContext();

			var gameOwner = await database
				.GameOwners.Include(q => q.Games)
				.FirstOrDefaultAsync(q => q.Id == id);
			return _mapper.Map<CommonModels.GameOwner>(gameOwner);
		}

		public async Task EditGameOwnerAsync(CommonModels.GameOwner commonGameOwner)
		{
			using var database = _contextFactory.GetContext();

			var gameOwner = await database
				.GameOwners.Include(q => q.Games)
				.FirstOrDefaultAsync(q => q.Id == commonGameOwner.Id);

			gameOwner.Id = commonGameOwner.Id;
			gameOwner.Name = commonGameOwner.Name;
			gameOwner.TGRef = commonGameOwner.TGRef;
			gameOwner.Games = _mapper.Map<ICollection<Game>>(commonGameOwner.Games);

			database.GameOwners.Update(gameOwner);
			await database.SaveChangesAsync();
		}

		public async Task<List<CommonModels.GameOwner>> GetAllGameOwnersAsync()
		{
			using var database = _contextFactory.GetContext();

			var gameOwnerList = database
				.GameOwners.Include(q => q.Games)
				.ToListAsync();
			return _mapper.Map<List<CommonModels.GameOwner>>(gameOwnerList);
		}
	}
}
