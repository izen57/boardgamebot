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
		private readonly BoardGameContext _boardGameContext;
		private readonly IMapper _mapper;

		public GameRepository(BoardGameContext boardGameContext, IMapper mapper)
		{
			_boardGameContext = boardGameContext ?? throw new ArgumentNullException(nameof(boardGameContext));
			_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
		}

		public async Task CreateGame(CommonModels.Game commonGame)
		{
			var game = _mapper.Map<Game>(commonGame);
			await _boardGameContext.AddAsync(game);
			await _boardGameContext.SaveChangesAsync();
		}

		public async Task<CommonModels.Game> GetGame(long id)
		{
			var game = await _boardGameContext
				.Games.Include(q => q.GameOwners)
				.FirstOrDefaultAsync(q => q.Id == id);
			return _mapper.Map<CommonModels.Game>(game);
		}

		public async Task EditGame(CommonModels.Game commonGame)
		{
			var game = await _boardGameContext
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

			_boardGameContext.Games.Update(game);
			await _boardGameContext.SaveChangesAsync();
		}

		public async Task<List<CommonModels.Game>> GetAllGames()
		{
			var gameList = _boardGameContext
				.Games.Include(q => q.GameOwners)
				.ToListAsync();
			return _mapper.Map<List<CommonModels.Game>>(gameList);
		}
	}
}
