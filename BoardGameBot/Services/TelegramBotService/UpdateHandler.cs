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
		private string _chatStatus;
		private CommonModels.Game _game;
		private CommonModels.Poll _poll;

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

			_chatStatus = "free";
			_game = new CommonModels.Game();
		}

		/**
		 * Обработка новых событий
		 */
		public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
		{
			_botClient = botClient;

			foreach (var group in await _groupRepository.GetAllGroupAsync())
			{
				if (group == null)
					break;
				else
					foreach (var poll in group.Polls)
						if (poll != null && poll.Time == DateTime.Now)
							await _botClient.SendPollAsync(
								_poll.GroupId,
								"Игры на сегодня",
								(await _gameRepository.GetAllGamesAsync())
									.Select(x => x.Title)
									.ToArray(),
								allowsMultipleAnswers: true,
								openPeriod: _poll.DayInterval
							);
						else
							break;
			}

			var handler = update.Type switch
			{
				UpdateType.Message =>
					BotOnMessageReceivedAsync(update.Message!),
				UpdateType.MyChatMember =>
					BotOnMyChatMemberAsync(update.MyChatMember!),
				UpdateType.CallbackQuery => BotOnCallbackQuery(update.CallbackQuery),
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
				var group = new CommonModels.Group(
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
		private async Task BotOnMessageReceivedAsync(Message message)
		{
			var handler = message.Type switch
			{
				MessageType.ChatMemberLeft =>
					BotOnLeftMemberAsync(message.LeftChatMember!),
				MessageType.ChatMembersAdded =>
					BotOnAddedMembersAsync(message.NewChatMembers!, message.Chat.Id),
				MessageType.Text => BotOnTextAsync(message!),
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
		private async Task BotOnTextAsync(Message message)
		{
			if (_chatStatus == "free")
			{
				var handler = message.Text switch
				{
					"@bgame1_bot" or "/start" or "/help"=> BotOnTagAsync(message),
					"/bg_adduser@bgame1_bot" or "/bg_adduser" => BotOnAddUserAsync(message),
					//"/bg_creategame@BoardGameQ_Bot" => BotOnCreateGame(message, update.CallbackQuery!),
					"/bg_creategame" => BotOnCreateGameAsync(message),
					"/bg_adduserongame" => BotOnAddGameAsync(message),
					"/bg_createpoll" => BotOnCreatePollAsync(message),
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
			else if (_chatStatus.Contains("Game"))
				await EnterGameByStatusAsync(message);
			else if (_chatStatus.Contains("Poll"))
				await EnterPollByStatusAsync(message);
		}

		/**
		 * Обработка событий от кнопок inline-клавиатуры.
		 */
		private async Task BotOnCallbackQuery(CallbackQuery callbackQuery)
		{
			var handler = callbackQuery.Data switch
			{
				"gametitle" => GameTitleMessageAsync(callbackQuery.Message),
				"gamedescr" => GameDescrMessageAsync(callbackQuery.Message),
				"gameplayers" => GamePlayersMessageAsync(callbackQuery.Message),
				"gamegenre" => GameGenreMessageAsync(callbackQuery.Message),
				"gamecomplexity" => GameComplMessageAsync(callbackQuery.Message),
				"gamelinks" => GameLinksMessageAsync(callbackQuery.Message),
				"gamerules" => GameRulesMessageAsync(callbackQuery.Message),
				"gamesave" => GameSaveAsync(callbackQuery.Message),
				"gameyes" => AddUserOnGameAsync(callbackQuery),
				"gameno" => _botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Игра уже создана."),
				string s when s.Contains("group") => AddUserAsync(callbackQuery),
				string s when s.Contains("game") => AddUserOnGameAsync(callbackQuery),
				"pollname" => PollNameMessageAsync(callbackQuery.Message),
				"polltime" => PollTimeMessageAsync(callbackQuery.Message),
				"pollinterval" => PollIntervalMessageAsync(callbackQuery.Message),
				"pollgroup" => PollGroupMessageAsync(callbackQuery.Message),
				"pollsave" => PollSaveMessageAsync(callbackQuery.Message)
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

		private async Task BotOnAddGameAsync(Message message)
		{
			var inlinelist = new List<InlineKeyboardButton>();

			foreach (var game in await _gameRepository.GetAllGamesAsync())
				inlinelist.Add(InlineKeyboardButton.WithCallbackData(game.Title, $"{game.Title}"));
			await _botClient.SendTextMessageAsync(
				message.Chat.Id,
				"Выберите игру из списка.",
				replyMarkup: new InlineKeyboardMarkup(inlinelist)
			);
		}

		private async Task BotOnCreatePollAsync(Message message)
		{
			if ((await _botClient.GetChatMemberAsync(message.Chat.Id, message.From.Id)).Status != ChatMemberStatus.Administrator)
			{
				await _botClient.SendTextMessageAsync(
					message.Chat.Id,
					"У Вас недостаточно прав для создания опроса."
				);
				return;
			}

			var keyboard = PollKeyboard();
			await _botClient.SendTextMessageAsync(
				message.Chat.Id,
				"Введите характеристики опроса.",
				replyMarkup: keyboard
			);
			_chatStatus = "Poll";
		}

		private async Task EnterPollByStatusAsync(Message message)
		{
			if (_chatStatus == "PollName")
			{
				_poll.Name = message.Text;

				var keyboard = PollKeyboard();
				await _botClient.SendTextMessageAsync(
					message.Chat.Id,
					"Введите характеристики опроса.",
					replyMarkup: keyboard
				);
			}
			else if (_chatStatus == "PollTime")
			{
				_poll.Time = Convert.ToDateTime(message.Text);

				var keyboard = PollKeyboard();
				await _botClient.SendTextMessageAsync(
					message.Chat.Id,
					"Введите характеристики опроса.",
					replyMarkup: keyboard
				);
			}
			else if (_chatStatus == "PollInterval")
			{
				_poll.DayInterval = int.Parse(message.Text);

				var keyboard = PollKeyboard();
				await _botClient.SendTextMessageAsync(
					message.Chat.Id,
					"Введите характеристики опроса.",
					replyMarkup: keyboard
				);
			}
			else if (_chatStatus == "PollGroup")
			{
				_poll.GroupId = message.Chat.Id;
				_poll.Group.Id = message.Chat.Id;
				_poll.Group.Name = message.Text;
				_poll.Group.Description = message.Chat.Description;
				_poll.Group.Members = null;
				_poll.Group.Admins = null;
				_poll.Group.Polls.Add(_poll);

				var keyboard = PollKeyboard();
				await _botClient.SendTextMessageAsync(
					message.Chat.Id,
					"Введите характеристики опроса.",
					replyMarkup: keyboard
				);
			}
		}

		private async Task EnterGameByStatusAsync(Message message)
		{
			if (_chatStatus == "GameTitle")
			{
				_game.Title = message.Text;

				var keyboard = GameKeyboard();
				await _botClient.SendTextMessageAsync(
					message.Chat.Id,
					"Введите характеристики создаваемой игры.",
					replyMarkup: keyboard
				);
			}
			else if (_chatStatus == "GameDescr")
			{
				_game.Description = message.Text;

				var keyboard = GameKeyboard();
				await _botClient.SendTextMessageAsync(
					message.Chat.Id,
					"Введите характеристики создаваемой игры.",
					replyMarkup: keyboard
				);
			}
			else if (_chatStatus == "GamePlayers")
			{
				_game.Players = message.Text;

				var keyboard = GameKeyboard();
				await _botClient.SendTextMessageAsync(
					message.Chat.Id,
					"Введите характеристики создаваемой игры.",
					replyMarkup: keyboard
				);
			}
			else if (_chatStatus == "GameGenre")
			{
				_game.Genre = message.Text;

				var keyboard = GameKeyboard();
				await _botClient.SendTextMessageAsync(
					message.Chat.Id,
					"Введите характеристики создаваемой игры.",
					replyMarkup: keyboard
				);
			}
			else if (_chatStatus == "GameComplexity")
			{
				_game.Complexity = int.Parse(message.Text);

				var keyboard = GameKeyboard();
				await _botClient.SendTextMessageAsync(
					message.Chat.Id,
					"Введите характеристики создаваемой игры.",
					replyMarkup: keyboard
				);
			}
			else if (_chatStatus == "GameLetsPlay")
			{
				_game.LetsPlay = message.Text;

				var keyboard = GameKeyboard();
				await _botClient.SendTextMessageAsync(
					message.Chat.Id,
					"Введите характеристики создаваемой игры.",
					replyMarkup: keyboard
				);
			}
			else if (_chatStatus == "GameRules")
			{
				_game.Rules = message.Text;

				var keyboard = GameKeyboard();
				await _botClient.SendTextMessageAsync(
					message.Chat.Id,
					"Введите характеристики создаваемой игры.",
					replyMarkup: keyboard
				);
			}
		}

		private async Task BotOnCreateGameAsync(Message message)
		{
			var keyboard = GameKeyboard();
			await _botClient.SendTextMessageAsync(
				message.Chat.Id,
				"Введите характеристики создаваемой игры.",
				replyMarkup: keyboard
			);
			_chatStatus = "Game";
		}

		private InlineKeyboardMarkup GameKeyboard()
		{
			return new(new[]
				{
					new[]
					{
						InlineKeyboardButton.WithCallbackData(_game.Title ?? "Название", "gametitle"),
						InlineKeyboardButton.WithCallbackData(_game.Description ?? "Описание", "gamedescr")
					},
					new[]
					{
						InlineKeyboardButton.WithCallbackData(_game.Players ?? "Количество игроков", "gameplayers")
					},
					new[]
					{
						InlineKeyboardButton.WithCallbackData(_game.Genre ?? "Жанр", "genre"),
						InlineKeyboardButton.WithCallbackData(_game.Complexity == -1 ? "Сложность" : _game.Complexity.ToString(), "gamecomplexity")
					},
					new[]
					{
						InlineKeyboardButton.WithCallbackData(_game.LetsPlay ?? "Полезные ссылки", "links"),
						InlineKeyboardButton.WithCallbackData(_game.Rules ?? "Правила", "gamerules")
					},
					new[]
					{
						InlineKeyboardButton.WithCallbackData("Сохранить", "gamesave")
					},
				}
			);
		}

		private InlineKeyboardMarkup PollKeyboard()
		{
			return new(new[]
				{
					new[]
					{
						InlineKeyboardButton.WithCallbackData(_poll.Name ?? "Название", "pollname"),
						InlineKeyboardButton.WithCallbackData(_poll.Time.ToString() ?? "Время опроса", "polltime")
					},
					new[]
					{
						InlineKeyboardButton.WithCallbackData(_poll.DayInterval == 0 ? "Дневной интервал" : _poll.DayInterval.ToString() , "pollinterval"),
						InlineKeyboardButton.WithCallbackData(_poll.Group.Name ?? "Группа опроса", "pollgroup")
					},
					new[]
					{
						InlineKeyboardButton.WithCallbackData("Сохранить опрос", "pollsave")
					}
				}
			);
		}

		private async Task GameTitleMessageAsync(Message message)
		{
			await _botClient.SendTextMessageAsync(message.Chat.Id, "Отправив ответ на это сообщение, введите название создаваемой игры.");

			_chatStatus = "GameTitle";
		}

		private async Task GameDescrMessageAsync(Message message)
		{
			await _botClient.SendTextMessageAsync(message.Chat.Id, "Отправив ответ на это сообщение, введите описание создаваемой игры.");

			_chatStatus = "GameDescr";
		}

		private async Task GamePlayersMessageAsync(Message message)
		{
			await _botClient.SendTextMessageAsync(message.Chat.Id, "Отправив ответ на это сообщение, введите, сколько игроков может играть в создаваемую игру.");

			_chatStatus = "GamePlayers";
		}

		private async Task GameGenreMessageAsync(Message message)
		{
			await _botClient.SendTextMessageAsync(message.Chat.Id, "Отправив ответ на это сообщение, введите жанр создаваемой игры.");

			_chatStatus = "GameGenre";
		}

		private async Task GameComplMessageAsync(Message message)
		{
			await _botClient.SendTextMessageAsync(message.Chat.Id, "Отправив ответ на это сообщение, введите сложность (цифра не меньше 0) создаваемой игры.");

			_chatStatus = "GameComplexity";
		}

		private async Task GameLinksMessageAsync(Message message)
		{
			await _botClient.SendTextMessageAsync(message.Chat.Id, "Отправив ответ на это сообщение, скопируйте ссылки на видео по создаваемой игры.");

			_chatStatus = "GameLetsPlay";
		}

		private async Task GameRulesMessageAsync(Message message)
		{
			await _botClient.SendTextMessageAsync(message.Chat.Id, "Отправив ответ на это сообщение, опишите правила создаваемой игры.");

			_chatStatus = "GameRules";
		}

		private async Task GameSaveAsync(Message message)
		{
			await _gameRepository.CreateGameAsync(_game);
			await _botClient.SendTextMessageAsync(
				message.Chat.Id,
				"Игра сохранена.\n\n" +
				$"Название: {_game.Title}\n" +
				$"Описание: {_game.Description}\n" +
				$"Количество игроков: {_game.Players}\n" +
				$"Жанр игры: {_game.Genre}\n" +
				$"Сложность: {_game.Complexity}\n" +
				$"Правила: {_game.Rules}\n" +
				$"Полезные ссылки: {_game.LetsPlay}\n" +
				$"Владельцы этой игры: {(_game.GameOwners == null ? "нету" : _game.GameOwners)}\n" +
				"Хотите стать одним из её владельцев (да/нет)?",
				replyMarkup: new InlineKeyboardMarkup(new[]
					{
						new[]
						{
							InlineKeyboardButton.WithCallbackData("Да", "gameyes"),
							InlineKeyboardButton.WithCallbackData("Нет", "gameno")
						}
					}
				)
			);

			_chatStatus = "free";
		}

		private async Task PollNameMessageAsync(Message message)
		{
			await _botClient.SendTextMessageAsync(message.Chat.Id, "Отправив ответ на это сообщение, введите название опроса.");

			_chatStatus = "PollName";
		}

		private async Task PollTimeMessageAsync(Message message)
		{
			await _botClient.SendTextMessageAsync(message.Chat.Id, "Отправив ответ на это сообщение, введите время отправки опроса.");

			_chatStatus = "PollTime";
		}

		private async Task PollIntervalMessageAsync(Message message)
		{
			await _botClient.SendTextMessageAsync(message.Chat.Id, "Отправив ответ на это сообщение, укажите сколько секунд будет открыт опрос (от 5 до 600).");

			_chatStatus = "PollInterval";
		}

		private async Task PollGroupMessageAsync(Message message)
		{
			await _botClient.SendTextMessageAsync(message.Chat.Id, "Отправив ответ на это сообщение, введите название группы, где будет проходить этот опрос.");

			_chatStatus = "PollGroup";
		}

		private async Task PollSaveMessageAsync(Message message)
		{
			await _pollRepository.CreatePollAsync(_poll);

			await _botClient.SendTextMessageAsync(
				message.Chat.Id,
				"Ваш опрос сохранён.\n\n" +
				$"Название: {_poll.Name}\n" +
				$"Время опроса: {_poll.Time}\n" +
				$"Опрос будет открытым в течение: {_poll.DayInterval}\n" +
				$"Группа, закреплённая за опросом: {_poll.Group.Name}\n"
			);

			_chatStatus = "free";
		}


		private async Task BotOnTagAsync(Message message)
		{
			await _botClient.SendTextMessageAsync(
				message.Chat.Id,
				"Это бот для организации совместных игр.\n\n" +
				"Напишите /bg_adduser для добавления пользователя к группе играющих, той в которой вызвали эту команду\n" +
				"Напишите /bg_creategame для создания новой игры\n" +
				"Напишите /bg_adduserongame, чтобы прикрепить пользователя к игре.\n" +
				"Напишите /bg_createpoll, чтобы создать повторяющийся опрос."
			);
		}

		private async Task BotOnAddUserAsync(Message message)
		{
			var inlinelist = new List<InlineKeyboardButton>();

			foreach (var group in await _groupRepository.GetAllGroupAsync())
				inlinelist.Add(InlineKeyboardButton.WithCallbackData(group.Name, $"group{group.Id}"));
			await _botClient.SendTextMessageAsync(
				message.Chat.Id,
				"Выберите группу из списка.",
				replyMarkup: new InlineKeyboardMarkup(inlinelist)
			);
		}

		private async Task AddUserAsync(CallbackQuery callback)
		{
			var groups = await _groupRepository.GetAllGroupAsync();
			if (groups.Any(g => g.Id == callback.Message.Chat.Id))
				return;

			var callbackId = new ChatId(callback.Data);
			var adminMembers = await _botClient.GetChatAdministratorsAsync(callbackId);
			var isMember = await _botClient.GetChatMemberAsync(callbackId, callback.Message.Chat.Id);
			if (isMember == null)
			{
				await _botClient.SendTextMessageAsync(callback.Message.Chat.Id, "Вы не состоите в этой группе.");
				return;
			}
			var adminMember = adminMembers.FirstOrDefault(m => m.User.Id == callback.Message.Chat.Id);

			long? adminMemberId = null;
			if (adminMember != null)
				adminMemberId = long.Parse(callbackId);
			var gameOwner = new CommonModels.GameOwner(
				callback.Message.Chat.Id,
				callback.Message.Chat.FirstName,
				adminMemberId,
				long.Parse(callbackId),
				callback.Message.Chat.Username,
				null,
				null,
				null
			);
			await _gameOwnerRepository.CreateGameOwnerAsync(gameOwner);
			await _botClient.SendTextMessageAsync(callback.Message.Chat.Id, ", Вы добавлены в группу.");
		}

		private async Task AddUserOnGameAsync(CallbackQuery callback)
		{
			var gameOwner = await _gameOwnerRepository.GetGameOwnerAsync(callback.From.Id);

			if (gameOwner == null)
			{
				await _botClient.SendTextMessageAsync(
					callback.Message.Chat.Id,
					"Вы не состоите в какой-либо группе." +
					"Введите команду /bg_adduser."
				);
				return;
			}

			_game.GameOwners.Add(gameOwner);
			await _gameRepository.EditGameAsync(_game);
			await _botClient.SendTextMessageAsync(
				callback.Message.Chat.Id,
				"Теперь Вы - владелец этой игры."
			);
			_chatStatus = "free";
		}

		private async Task BotOnLeftMemberAsync(User member)
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
					var gameOwner = new CommonModels.GameOwner(
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
