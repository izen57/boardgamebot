using BoardGameBot.Database.Adapter.Repositories.Interfaces;
using CommonModels = BoardGameBot.Models;

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
				"/bg_adduser@BoardGameQ_Bot" => BotOnAddUser(message, update.CallbackQuery!),
				"/bg_adduser" => BotOnAddUser(message, update.CallbackQuery!),
				// "/bg_creategame@BoardGameQ_Bot" => BotOnCreateGame(message, update.CallbackQuery!),
				// "/bg_creategame" => BotOnCreateGame(message, update.CallbackQuery!),
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

		private async Task BotOnCreateGame(Message message, CallbackQuery callback)
		{
			InlineKeyboardMarkup keyboard = new(new[]
				{
					new[]
					{
						InlineKeyboardButton.WithCallbackData("Название", "title"),
						InlineKeyboardButton.WithCallbackData("Описание", "descr")
					},
					new[]
					{
						InlineKeyboardButton.WithCallbackData("Количество игроков", "players")
					},
					new[]
					{
						InlineKeyboardButton.WithCallbackData("Жанр", "genre"),
						InlineKeyboardButton.WithCallbackData("Сложность", "complexity")
					},
					new[]
					{
						InlineKeyboardButton.WithCallbackData("Полезные ссылки", "links"),
						InlineKeyboardButton.WithCallbackData("Правила", "rules")
					},
					new[]
					{
						InlineKeyboardButton.WithCallbackData("Сохранить", "save")
					},
					new[]
					{
						InlineKeyboardButton.WithCallbackData("Назад", "back")
					}
				}
			);

			string? inputGameTitle = null;
			string? inputGameDescription = null;
			string? inputGamePlayers = null;
			string? inputGameGenre = null;
			string? inputGameComplexity = null;
			string? inputGameLinks = null;
			string? inputGameRules = null;

			var handler = callback.Data switch
			{
				"title" => inputGameTitle = await GameNameEnterAsync(message, keyboard),
				//"descr" => inputGameDescription = GameDescrEnterAsync(message, keyboard),
				//"players" => inputGamePlayers = GamePlayersEnterAsync(message, keyboard),
				//"genre" => inputGameGenre = GameGenreEnterAsync(message, keyboard),
				//"complexity" => inputGameComplexity = GameComplEnterAsync(message, keyboard),
				//"links" => inputGameLinks = GameLinksEnterAsync(message, keyboard),
				//"rules" => inputGameRules = GameRulesEnterAsync(message, keyboard),
				//"save" => GameSave(message, keyboard),
				//"back" => KeyaboardReturn(message, keyboard)
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

		private async Task<string> GameNameEnterAsync(Message message, InlineKeyboardMarkup keyboard)
		{
			await _botClient.SendTextMessageAsync(message.Chat.Id, "Отправив ответ на это сообщение, введите название новой игры.");

			//var gameName = await _botClient.get
			return null;
		}

		private async Task BotOnAddUser(Message message, CallbackQuery callback)
		{
			var inlinelist = new List<InlineKeyboardButton>();

			foreach (var group in await _groupRepository.GetAllGroupAsync())
				inlinelist.Add(InlineKeyboardButton.WithCallbackData(group.Name, $"group{group.Id}"));
			await _botClient.SendTextMessageAsync(
				message.Chat.Id,
				"Выберите группу из списка",
				replyMarkup: new InlineKeyboardMarkup(inlinelist)
			);

			var groups = await _groupRepository.GetAllGroupAsync();
			if (groups.Any(g => g.Id == message.Chat.Id))
				return;

			var callbackId = new ChatId(callback.Data);
			var adminMembers = await _botClient.GetChatAdministratorsAsync(callbackId);
			var isMember = await _botClient.GetChatMemberAsync(callbackId, message.Chat.Id);
			if (isMember == null)
			{
				await _botClient.SendTextMessageAsync(message.Chat.Id, "Вы не состоите в этой группе.");
				return;
			}
			var adminMember = adminMembers.FirstOrDefault(m => m.User.Id == message.Chat.Id);

			long? adminMemberId = null;
			if (adminMember != null)
				adminMemberId = long.Parse(callbackId);
			var gameOwner = new GameOwner(
				message.Chat.Id,
				message.Chat.FirstName,
				adminMemberId,
				long.Parse(callbackId),
				message.Chat.Username,
				null,
				null,
				null
			);
			await _gameOwnerRepository.CreateGameOwnerAsync(gameOwner);
			await _botClient.SendTextMessageAsync(message.Chat.Id, ", Вы добавлены в группу.");
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
