using BoardGameBot.Core;

using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;

using TelegramBotService.Configuration;

namespace TelegramBotService
{
	public class TelegramBotService: ITelegramBotService
	{
		private bool _isAbortRequested = false;
		private Thread _backgroungThread;
		private readonly ITelegramBotConfiguration _configuration;

		public TelegramBotService(ITelegramBotConfiguration configuration)
		{
			_configuration = configuration;
		}

		public Thread StartAsync()
		{
			if (_backgroungThread != null)
				StopAsync();

			_backgroungThread = new Thread(ThreadBody);
			_backgroungThread.IsBackground = true;
			_backgroungThread.Name = "TelegramBot";
			_backgroungThread.Start();
			return _backgroungThread;
		}

		public void StopAsync()
		{
			if (_backgroungThread != null)
			{
				_isAbortRequested = true;
				_backgroungThread = null;
			}
		}

		private void ThreadBody()
		{
			var bot = new TelegramBotClient(_configuration.Token);
			var receiverOptions = new ReceiverOptions
			{
				AllowedUpdates = Array.Empty<UpdateType>(),
				ThrowPendingUpdates = true
			};
			using var cts = new CancellationTokenSource();
			var updateHadler = new UpdateHadler();
			bot.StartReceiving(
				updateHandler: updateHadler.HandleUpdateAsync,
				pollingErrorHandler: updateHadler.HandlePollingErrorAsync,
				receiverOptions: receiverOptions,
				cancellationToken: cts.Token
			);

			while (!_isAbortRequested)
			{
			}

			cts.Cancel();
		}
	}
}