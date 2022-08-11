using BoardGameBot.Models;

namespace BoardGameBot.Database.Adapter.Repositories.Interfaces
{
	public interface IGroupRepository
	{
		Task CreateGroup(Group commonGroup);
		Task<Group> GetGroup(long id);
		Task EditGroup(Group commonGroup);
		Task<List<Group>> GetAllGroup();
	}
}
