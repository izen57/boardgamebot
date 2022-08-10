using Microsoft.Extensions.Hosting;

namespace BoardGameBot.Core
{
	public class CoreService: IHostedService
	{
		public Task StartAsync(CancellationToken cancellationToken)
		{
			return Task.CompletedTask;
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			return Task.CompletedTask;
		}
	}
}