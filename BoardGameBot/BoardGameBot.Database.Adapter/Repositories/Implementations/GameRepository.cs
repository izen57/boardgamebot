using AutoMapper;

using BoardGameBot.Database.Adapter.Repositories.Interfaces;
using BoardGameBot.Database.PostgreSQL;
using BoardGameBot.Database.PostgreSQL.Models;

using Microsoft.EntityFrameworkCore;

using CommonModels = BoardGameBot.Models;

namespace BoardGameBot.Database.Adapter.Repositories.Implementations
{
	public class GameRepository: IGameRepository
	{
		private readonly IContextFactory _contextFactory;
		private readonly IMapper _mapper;

		public GameRepository(IContextFactory contextFactory, IMapper mapper)
		{
			_contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
			_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
		}

		public async Task CreateGameAsync(CommonModels.Game commonGame)
		{
			using var database = _contextFactory.GetContext();

			var game = _mapper.Map<Game>(commonGame);
			await database.Games.AddAsync(game);
			await database.SaveChangesAsync();
		}

		public async Task<CommonModels.Game> GetGameAsync(long id)
		{
			using var database = _contextFactory.GetContext();

			var game = await database
				.Games.Include(q => q.GameOwners)
				.FirstOrDefaultAsync(q => q.Id == id);
			return _mapper.Map<CommonModels.Game>(game);
		}

		public async Task EditGameAsync(CommonModels.Game commonGame)
		{
			using var database = _contextFactory.GetContext();

			var game = await database
				.Games.Include(q => q.GameOwners)
				.FirstOrDefaultAsync(q => q.Id == commonGame.Id);

			game.Id = commonGame.Id;
			game.Title = commonGame.Title;
			game.Description = commonGame.Description;
			game.Players = commonGame.Players;
			game.Genre = commonGame.Genre;
			game.Complexity = commonGame.Complexity;
			game.LetsPlay = commonGame.LetsPlay;
			game.Rules = commonGame.Rules;
			game.Played = commonGame.Played;
			game.GameOwners = _mapper.Map<ICollection<GameOwner>>(commonGame.GameOwners);

			database.Games.Update(game);
			await database.SaveChangesAsync();
		}

		public async Task<List<CommonModels.Game>> GetAllGamesAsync()
		{
			using var database = _contextFactory.GetContext();

			var gameList = database
				.Games.Include(q => q.GameOwners)
				.ToListAsync();
			return _mapper.Map<List<CommonModels.Game>>(gameList);
		}
	}
}
