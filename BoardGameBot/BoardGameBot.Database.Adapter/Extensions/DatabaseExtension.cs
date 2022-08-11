using BoardGameBot.Database.Adapter.Repositories.Implementations;
using BoardGameBot.Database.Adapter.Repositories.Interfaces;

using Microsoft.Extensions.DependencyInjection;

namespace BoardGameBot.Database.Adapter.Extensions
{
	public static class DatabaseExtension
	{
		public static IServiceCollection AddRepositories(this IServiceCollection services)
		{
			// почему временные сервисы, а не по области применения
			services.AddTransient<IGameOwnerRepository, GameOwnerRepository>();
			services.AddTransient<IGameRepository, GameRepository>();
			return services;
		}
	}
}
