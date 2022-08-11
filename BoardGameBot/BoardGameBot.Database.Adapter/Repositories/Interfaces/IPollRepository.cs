using BoardGameBot.Models;

namespace BoardGameBot.Database.Adapter.Repositories.Interfaces
{
	public interface IPollRepository
	{
		Task CreatePoll(Poll commonPoll);
		Task<Poll> GetPoll(long id);
		Task EditPoll(Poll commonPoll);
		Task<List<Poll>> GetAllPolls();
	}
}
