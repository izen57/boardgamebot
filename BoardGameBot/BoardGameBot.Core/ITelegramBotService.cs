namespace BoardGameBot.Core
{
	public interface ITelegramBotService
	{
		Thread StartAsync();
		void StopAsync();
	}
}
