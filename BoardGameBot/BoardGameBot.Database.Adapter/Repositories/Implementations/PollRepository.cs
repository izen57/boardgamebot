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
		private readonly IContextFactory _contextFactory;
		private readonly IMapper _mapper;

		public PollRepository(IContextFactory contextFactor, IMapper mapper)
		{
			_contextFactory = contextFactor ?? throw new ArgumentNullException(nameof(contextFactor));
			_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
		}

		public async Task CreatePollAsync(CommonModels.Poll commonPoll)
		{
			using var database = _contextFactory.GetContext();

			var poll = _mapper.Map<Poll>(commonPoll);
			await database.AddAsync(poll);
			await database.SaveChangesAsync();
		}

		public async Task EditPollAsync(CommonModels.Poll commonPoll)
		{
			using var database = _contextFactory.GetContext();

			var poll = await database
				.Polls
				.Include(q => q.Group)
				.FirstOrDefaultAsync(q => q.Id == commonPoll.Id);

			poll.Id = commonPoll.Id;
			poll.Name = commonPoll.Name;
			poll.Time = commonPoll.Time;
			poll.Group = _mapper.Map<Group>(commonPoll.Group);

			database.Polls.Update(poll);
			await database.SaveChangesAsync();
		}

		public async Task<List<CommonModels.Poll>> GetAllPollsAsync()
		{
			using var database = _contextFactory.GetContext();

			var pollList = await database
				.Polls
				.Include(q => q.Group)
				.ToListAsync();
			return _mapper.Map<List<CommonModels.Poll>>(pollList);
		}

		public async Task<CommonModels.Poll> GetPollAsync(long id)
		{
			using var database = _contextFactory.GetContext();

			var poll = await database
				.Polls
				.Include(q => q.Group)
				.FirstOrDefaultAsync(q => q.Id == id);
			return _mapper.Map<CommonModels.Poll>(poll);
		}
	}
}
