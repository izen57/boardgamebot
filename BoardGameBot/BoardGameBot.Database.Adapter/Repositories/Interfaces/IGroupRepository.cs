using BoardGameBot.Models;

namespace BoardGameBot.Database.Adapter.Repositories.Interfaces
{
	public interface IGroupRepository
	{
		Task CreateGroupAsync(Group commonGroup);
		Task<Group> GetGroupAsync(long id);
		Task EditGroupAsync(Group commonGroup);
		Task<List<Group>> GetAllGroupAsync();
		Task<bool> DeleteGroupAsync(long id);
	}
}
