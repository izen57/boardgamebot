using BoardGameBot.Models;

namespace BoardGameBot.Database.Adapter.Repositories.Interfaces
{
	public interface IPollRepository
	{
		Task CreatePollAsync(Poll commonPoll);
		Task<Poll> GetPollAsync(long id);
		Task EditPollAsync(Poll commonPoll);
		Task<List<Poll>> GetAllPollsAsync();
	}
}
