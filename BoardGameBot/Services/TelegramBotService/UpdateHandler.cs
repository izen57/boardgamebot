﻿using BoardGameBot.Database.Adapter.Repositories.Interfaces;
using BoardGameBot.Models;

using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

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

		public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
		{
			_botClient = botClient;
			var handler = update.Type switch
			{
				UpdateType.Message =>
					BotOnMessageReceivedAsync(update.Message!),
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

		private async Task BotOnMyChatMemberAsync(ChatMemberUpdated chatMember)
		{
			if (chatMember.NewChatMember.Status == ChatMemberStatus.Left || chatMember.NewChatMember.Status == ChatMemberStatus.Kicked)
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

		private async Task BotOnMessageReceivedAsync(Message message)
		{
			if (message.LeftChatMember is { } leftChatMember)
			{
				var chatId = message.Chat.Id;

				Console.WriteLine($"User {leftChatMember.Id} left in chat {chatId}.");
				//var gameOwner = new GameOwner(
				//	chat
				//)
				//var gameOwnerRepository.
			}
			if (message.NewChatMembers is { } newChatMembers)
			{
				var chatId = message.Chat.Id;
				foreach (var newChatMember in newChatMembers)
				{
					Console.WriteLine($"User {newChatMember.Id} new in chat {chatId}.");
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
