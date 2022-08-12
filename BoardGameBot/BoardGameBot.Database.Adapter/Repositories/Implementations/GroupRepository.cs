using AutoMapper;

using BoardGameBot.Database.Adapter.Repositories.Interfaces;
using BoardGameBot.Database.PostgreSQL;
using BoardGameBot.Database.PostgreSQL.Models;

using Microsoft.EntityFrameworkCore;

using CommonModels = BoardGameBot.Models;

namespace BoardGameBot.Database.Adapter.Repositories.Implementations
{
	public class GroupRepository: IGroupRepository
	{
		private readonly BoardGameContext _boardGameContext;
		private readonly IMapper _mapper;

		public GroupRepository(BoardGameContext boardGameContext, IMapper mapper)
		{
			_boardGameContext = boardGameContext ?? throw new ArgumentNullException(nameof(boardGameContext));
			_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
		}

		public async Task CreateGroup(CommonModels.Group commonGroup)
		{
			var group = _mapper.Map<Group>(commonGroup);
			await _boardGameContext.AddAsync(group);
			await _boardGameContext.SaveChangesAsync();
		}

		public async Task EditGroup(CommonModels.Group commonGroup)
		{
			var group = await _boardGameContext
				.Groups
				.Include(q => q.Members)
				.Include(q => q.Polls)
				.FirstOrDefaultAsync(q => q.Id == commonGroup.Id);

			group.Id = commonGroup.Id;
			group.Description = commonGroup.Description;
			group.Members = _mapper.Map<ICollection<GameOwner>>(commonGroup.Members);
			group.Admins = _mapper.Map<ICollection<GameOwner>>(commonGroup.Admins);
			group.Polls = _mapper.Map<ICollection<Poll>>(commonGroup.Polls);

			_boardGameContext.Groups.Update(group);
			await _boardGameContext.SaveChangesAsync();
		}

		public async Task<List<CommonModels.Group>> GetAllGroup() {
			var groupList = await _boardGameContext
				.Groups
				.Include(q => q.Members)
				.Include(q => q.Polls)
				.ToListAsync();
			return _mapper.Map<List<CommonModels.Group>>(groupList);
		}

		public async Task<CommonModels.Group> GetGroup(long id)
		{
			var group = await _boardGameContext
				.Groups
				.Include(q => q.Members)
				.Include(q => q.Polls)
				.FirstOrDefaultAsync(q => q.Id == id);
			return _mapper.Map<CommonModels.Group>(group);
		}
	}
}
