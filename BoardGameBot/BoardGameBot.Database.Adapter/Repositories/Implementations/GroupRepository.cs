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
		private readonly IContextFactory _contextFactory;
		private readonly IMapper _mapper;

		public GroupRepository(IContextFactory contextFactory, IMapper mapper)
		{
			_contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
			_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
		}

		public async Task CreateGroupAsync(CommonModels.Group commonGroup)
		{
			using var database = _contextFactory.GetContext();

			var group = _mapper.Map<Group>(commonGroup);
			await database.Groups.AddAsync(group);
			await database.SaveChangesAsync();
		}

		public async Task<bool> DeleteGroupAsync(long id)
		{
			using var database = _contextFactory.GetContext();

			var group = await database
				.Groups
				.FirstOrDefaultAsync(q => q.Id == id);
			if (group != null)
			{
				database.Groups.Remove(group);
				await database.SaveChangesAsync();
				return true;
			}
			return false;
		}

		public async Task EditGroupAsync(CommonModels.Group commonGroup)
		{
			using var database = _contextFactory.GetContext();

			var group = await database
				.Groups
				.Include(q => q.Members)
				.Include(q => q.Polls)
				.FirstOrDefaultAsync(q => q.Id == commonGroup.Id);

			group.Id = commonGroup.Id;
			group.Description = commonGroup.Description;
			group.Members = _mapper.Map<ICollection<GameOwner>>(commonGroup.Members);
			group.Admins = _mapper.Map<ICollection<GameOwner>>(commonGroup.Admins);
			group.Polls = _mapper.Map<ICollection<Poll>>(commonGroup.Polls);

			database.Groups.Update(group);
			await database.SaveChangesAsync();
		}

		public async Task<List<CommonModels.Group>> GetAllGroupAsync() {
			using var database = _contextFactory.GetContext();

			var groupList = await database
				.Groups
				.Include(q => q.Members)
				.Include(q => q.Polls)
				.ToListAsync();
			return _mapper.Map<List<CommonModels.Group>>(groupList);
		}

		public async Task<CommonModels.Group> GetGroupAsync(long id)
		{
			using var database = _contextFactory.GetContext();

			var group = await database
				.Groups
				.Include(q => q.Members)
				.Include(q => q.Polls)
				.FirstOrDefaultAsync(q => q.Id == id);
			return _mapper.Map<CommonModels.Group>(group);
		}
	}
}
