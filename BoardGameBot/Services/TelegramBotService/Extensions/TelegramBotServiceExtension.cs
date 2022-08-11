using BoardGameBot.Core;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using TelegramBotService.Configuration;

namespace TelegramBotService.Extensions
{
	public static class TelegramBotServiceExtension
	{
		public static void AddTelegramBotService(this IServiceCollection services, IConfiguration configuration)
		{
			var section = configuration.GetSection("TelegramBot");
			var config = section.Get<TelegramBotConfiguration>();
			services.AddTransient<ITelegramBotConfiguration>(service => config);
			services.AddSingleton<ITelegramBotService, TelegramBotService>();
		}
	}
}
