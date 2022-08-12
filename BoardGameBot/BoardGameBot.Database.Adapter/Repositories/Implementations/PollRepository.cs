using AutoMapper;

using BoardGameBot.Database.Adapter.Repositories.Interfaces;
using BoardGameBot.Database.PostgreSQL;
using BoardGameBot.Database.PostgreSQL.Models;

using Microsoft.EntityFrameworkCore;

using CommonModels = BoardGameBot.Models;

namespace BoardGameBot.Database.Adapter.Repositories.Implementations
{
	public class PollRepository: IPollRepository
	{
		private readonly BoardGameContext _boardGameContext;
		private readonly IMapper _mapper;

		public PollRepository(BoardGameContext boardGameContext, IMapper mapper)
		{
			_boardGameContext = boardGameContext ?? throw new ArgumentNullException(nameof(boardGameContext));
			_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
		}

		public async Task CreatePoll(CommonModels.Poll commonPoll)
		{
			var poll = _mapper.Map<Poll>(commonPoll);
			await _boardGameContext.AddAsync(poll);
			await _boardGameContext.SaveChangesAsync();
		}

		public async Task EditPoll(CommonModels.Poll commonPoll)
		{
			var poll = await _boardGameContext
				.Polls
				.Include(q => q.Group)
				.FirstOrDefaultAsync(q => q.Id == commonPoll.Id);

			poll.Id = commonPoll.Id;
			poll.Name = commonPoll.Name;
			poll.Time = commonPoll.Time;
			poll.Group = _mapper.Map<Group>(commonPoll.Group);

			_boardGameContext.Polls.Update(poll);
			await _boardGameContext.SaveChangesAsync();
		}

		public async Task<List<CommonModels.Poll>> GetAllPolls()
		{
			var pollList = await _boardGameContext
				.Polls
				.Include(q => q.Group)
				.ToListAsync();
			return _mapper.Map<List<CommonModels.Poll>>(pollList);
		}

		public async Task<CommonModels.Poll> GetPoll(long id)
		{
			var poll = await _boardGameContext
				.Polls
				.Include(q => q.Group)
				.FirstOrDefaultAsync(q => q.Id == id);
			return _mapper.Map<CommonModels.Poll>(poll);
		}
	}
}
