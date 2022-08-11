using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;

namespace TelegramBotService
{
	public class UpdateHadler
	{
		// 0. добавить модель группы (id группы, создан ли опрос (тоже таблица(время, день, название опроса, ...), связь одна группа ко многим опросам), название группы, описание, список всех участников (один ко многим на GameOwner), список всех админов группы (один ко многим на GameOwner))
		// 0.1. команда привязать бота к группе (заполняется id группы и название, описание)
		// 1. получить инфу об участниках чата и записать их в БД через getAllUsers
		// 2. писать команды создания игры через inline-клавиатуру (добавить новую или выбрать из существующих игр, в ней кнопки с игроками, по которым можно назначить игрока, потом кнопки с полями модели (название, описание, жанр))
		// 3. создание опроса по времени (только админу через сравнение по таблице ник того, кто пишет). Если он админ, то создание опроса, иначе "недостаточно прав".

		public UpdateHadler()
		{
		}

		public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
		{
			// Only process Message updates: https://core.telegram.org/bots/api#message
			if (update.Message is not { } message) //editedMessage
				return;
			// Only process text messages
			if (message.Text is not { } messageText)
				return;

			if (messageText.Contains("BG"))
			{
			var chatId = message.Chat.Id;

			Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");

			// Echo received message text
			Message sentMessage = await botClient.SendTextMessageAsync(
				chatId: chatId,
				text: "You said:\n" + messageText,
				cancellationToken: cancellationToken);
			}
		}

		public Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
		{
			var ErrorMessage = exception switch
			{
				ApiRequestException apiRequestException
					=> $"Telegram API Error:\n" +
					$"[{apiRequestException.ErrorCode}]\n" +
					$"{apiRequestException.Message}",
				_ => exception.ToString()
			};

			Console.WriteLine(ErrorMessage);
			return Task.CompletedTask;
		}
	}
}
