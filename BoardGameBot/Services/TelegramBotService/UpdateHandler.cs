using BoardGameBot.Database.Adapter.Repositories.Interfaces;
using BoardGameBot.Models;

using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBotService
{
	public class UpdateHandler
	{
		// 0. добавить модель группы (id группы, создан ли опрос (тоже таблица(время, день, название опроса, ...), связь одна группа ко многим опросам), название группы, описание, список всех участников (один ко многим на GameOwner), список всех админов группы (один ко многим на GameOwner))
		// 0.1. команда привязать бота к группе (заполняется id группы и название, описание)
		// 1. получить инфу об участниках чата и записать их в БД через getAllUsers
		// 2. писать команды создания игры через inline-клавиатуру (добавить новую или выбрать из существующих игр, в ней кнопки с игроками, по которым можно назначить игрока, потом кнопки с полями модели (название, описание, жанр))
		// 3. создание опроса по времени (только админу через сравнение по таблице ник того, кто пишет). Если он админ, то создание опроса, иначе "недостаточно прав".
		private ITelegramBotClient _botClient;
		private IGameOwnerRepository _gameOwnerRepository;
		private IGameRepository _gameRepository;
		private IGroupRepository _groupRepository;
		private IPollRepository _pollRepository;

		public UpdateHandler(IGameOwnerRepository gameOwnerRepository,
			IGameRepository gameRepository,
			IGroupRepository groupRepository,
			IPollRepository pollRepository
		)
		{
			_gameOwnerRepository = gameOwnerRepository;
			_gameRepository = gameRepository;
			_groupRepository = groupRepository;
			_pollRepository = pollRepository;
		}

		/**
		 * Обработка новых событий
		 */
		public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
		{
			_botClient = botClient;
			var handler = update.Type switch
			{
				//UpdateType.te
				UpdateType.Message =>
					BotOnMessageReceivedAsync(update.Message!, update),
				UpdateType.MyChatMember =>
					BotOnMyChatMemberAsync(update.MyChatMember!),
			};
			try
			{
				await handler;
			}
			catch (Exception exception)
			{
				await HandlePollingErrorAsync(botClient, exception, cancellationToken);
			}
		}

		/**
		 * Обработка событий, произошедших с самими ботом. Например,
		 * когда он покидает группу или его исключают оттуда.
		 */
		private async Task BotOnMyChatMemberAsync(ChatMemberUpdated chatMember)
		{
			if (chatMember.NewChatMember.Status == ChatMemberStatus.Left
				|| chatMember.NewChatMember.Status == ChatMemberStatus.Kicked)
				await _groupRepository.DeleteGroupAsync(chatMember.Chat.Id);

			if (chatMember.NewChatMember.Status == ChatMemberStatus.Member)
			{
				var group = new Group(
					chatMember.Chat.Id,
					chatMember.Chat.Title,
					chatMember.Chat.Description,
					null,
					null,
					null
				);
				await _groupRepository.CreateGroupAsync(group);
			}
		}

		/**
		 * Обработка событий ботом в самом чате. Например: кто-то зашёл в группу или вышел оттуда, боту дали команду.
		 */
		private async Task BotOnMessageReceivedAsync(Message message, Update update)
		{
			var handler = message.Type switch
			{
				MessageType.ChatMemberLeft =>
					BotOnLeftMemberAsync(message.LeftChatMember!, message.Chat.Id),
				MessageType.ChatMembersAdded =>
					BotOnAddedMembersAsync(message.NewChatMembers!, message.Chat.Id),
				MessageType.Text => BotOnTextAsync(message!, update)
			};
			try
			{
				await handler;
			}
			catch (Exception exception)
			{
				throw exception;
			}
		}

		/**
		 * Обработка текстового сообщения, присланного боту.
		 */
		private async Task BotOnTextAsync(Message message, Update update)
		{
			var handler = message.Text switch
			{
				"@BoardGameQ_Bot" => BotOnTagAsync(message),
				"/bg_adduser@BoardGameQ_Bot" => BotOnAddUser(message, update.CallbackQuery!)
			};
			try
			{
				await handler;
			}
			catch (Exception exception)
			{
				throw exception;
			}
		}

		private async Task BotOnTagAsync(Message message)
		{
			await _botClient.SendTextMessageAsync(message.Chat.Id, $"Привет, {message.From.FirstName}.");
		}

		private async Task BotOnAddUser(Message message, CallbackQuery callback)
		{
			InlineKeyboardMarkup addUserKeyboard = new(new[]
				{
					new[]
					{
						InlineKeyboardButton.WithCallbackData("ID", "UserId")
					},
					new []
					{
						InlineKeyboardButton.WithCallbackData("Имя", "UserUsername")
					},
					new []
					{
						InlineKeyboardButton.WithCallbackData("Тэг", "UserFirstName")
					},
					new []
					{
						InlineKeyboardButton.WithCallbackData("ID группы,\nв которой состоит пользователь", "GroupId")
					}
				}
			);
			await _botClient.SendTextMessageAsync(
				message.Chat.Id,
				"Введите данные пользователя",
				replyToMessageId: message.MessageId,
				replyMarkup: addUserKeyboard
			);

			var handler = callback.Data switch
			{
				"UserId" => inlineUserId()
			};
			try
			{
				await handler;
			}
			catch (Exception exception)
			{
				throw exception;
			}
			//foreach (var group in await _groupRepository.GetAllGroupAsync())
			//{
			//	//выбор группы через inline
			//	message.edit
			//}
			//var groups = await _groupRepository.GetAllGroupAsync();
			//if (groups.Any(g => g.Id == message.Chat.Id))
			//	return;

			//var adminMembers = await _botClient.GetChatAdministratorsAsync(/*получения от inline-клавы от пользователя, в какую группу добавить*/);
			//var isMember = await _botClient.GetChatMembersAsync(/*получения от inline-клавы от пользователя, в какую группу добавить*/, message.ForwardFrom.Id);
			//if (!isMember)
			//{
			//	await _botClient.SendTextMessageAsync(message.Chat.Id, "не состоите в группе");
			//}
			//var adminMember = adminMembers.FirstOrDefault(m => m.User.Id == message.ForwardFrom?.Id);

			//	long? adminMemberId = null;
			//	if (adminMember != null)
			//		adminMemberId = /*получения от inline-клавы от пользователя, в какую группу добавить*/;
			//	var gameOwner = new GameOwner(
			//		message.ForwardFrom.Id,
			//		message.ForwardFrom.FirstName,
			//		adminMemberId,
			//		/*получения от inline-клавы от пользователя, в какую группу добавить*/
			//		message.ForwardFrom.Username,
			//		null,
			//		null,
			//		null
			//	);
			//	await _gameOwnerRepository.CreateGameOwnerAsync(gameOwner);

		}

		private async Task inlineUserId()
		{
			throw new NotImplementedException();
		}

		private async Task BotOnLeftMemberAsync(User member, long groupId)
		{
			await _gameOwnerRepository.DeleteGameOwnerAsync(member.Id);
		}

		private async Task BotOnAddedMembersAsync(User[] members, long groupId)
		{
			var adminMembers = await _botClient.GetChatAdministratorsAsync(groupId);
			foreach (var member in members)
			{
				if (!member.IsBot)
				{
					var adminMember = adminMembers.FirstOrDefault(m => m.User.Id == member.Id);

					long? adminMemberId = null;
					if (adminMember != null)
						adminMemberId = groupId;
					var gameOwner = new GameOwner(
						member.Id,
						member.FirstName,
						adminMemberId,
						groupId,
						member.Username,
						null,
						null,
						null
					);
					await _gameOwnerRepository.CreateGameOwnerAsync(gameOwner);
				}
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
