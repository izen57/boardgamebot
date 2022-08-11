using Microsoft.Extensions.Hosting;

namespace BoardGameBot.Core
{
	public class CoreService: IHostedService
	{
		private readonly ITelegramBotService _service;
		public CoreService(ITelegramBotService service)
		{
			_service = service;
		}

		public Task StartAsync(CancellationToken cancellationToken)
		{
			_service.StartAsync();
			return Task.CompletedTask;
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			_service.StopAsync();
			return Task.CompletedTask;
		}
	}
}