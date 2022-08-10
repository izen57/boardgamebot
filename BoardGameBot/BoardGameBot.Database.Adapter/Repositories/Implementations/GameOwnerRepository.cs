using AutoMapper;

using BoardGameBot.Database.Adapter.Repositories.Interfaces;
using BoardGameBot.Database.PostgreSQL;
using BoardGameBot.Database.PostgreSQL.Models;

using Microsoft.EntityFrameworkCore;

using Models = BoardGameBot.Models;


namespace BoardGameBot.Database.Adapter.Repositories.Implementations
{
	public class GameOwnerRepository: IGameOwnerRepository
	{
		private readonly BoardGameContext _boardGameContext;
		private readonly IMapper _mapper;

		public GameOwnerRepository(BoardGameContext boardGameContext, IMapper mapper)
		{
			_boardGameContext = boardGameContext ?? throw new ArgumentNullException(nameof(boardGameContext));
			_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
		}

		public async Task CreateGameOwner(Models.GameOwner commonGameOwner)
		{
			var gameOwner = _mapper.Map<GameOwner>(commonGameOwner);
			await _boardGameContext.AddAsync(gameOwner);
			await _boardGameContext.SaveChangesAsync();
		}

		public async Task<Models.GameOwner> GetGameOwner(long id)
		{
			var gameOwner = await _boardGameContext
				.GameOwners.Include(q => q.Games)
				.FirstOrDefaultAsync(q => q.Id == id);
			return _mapper.Map<Models.GameOwner>(gameOwner);
		}

		public async Task EditGameOwner(Models.GameOwner commonGameOwner)
		{
			var gameOwner = await _boardGameContext
				.GameOwners.Include(q => q.Games)
				.FirstOrDefaultAsync(q => q.Id == commonGameOwner.Id);

			gameOwner.Id = commonGameOwner.Id;
			gameOwner.Name = commonGameOwner.Name;
			gameOwner.TGRef = commonGameOwner.TGRef;
			gameOwner.Games = _mapper.Map<ICollection<Game>>(commonGameOwner.Games);

			_boardGameContext.GameOwners.Update(gameOwner);
			await _boardGameContext.SaveChangesAsync();
		}

		public async Task<List<Models.GameOwner>> GetAllGameOwner()
		{
			var gameOwnerList = _boardGameContext
				.GameOwners.Include(q => q.Games)
				.ToListAsync();
			return _mapper.Map<List<Models.GameOwner>>(gameOwnerList);
		}
	}
}
