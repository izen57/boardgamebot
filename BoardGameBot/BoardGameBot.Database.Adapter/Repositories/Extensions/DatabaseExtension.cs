using BoardGameBot.Database.Adapter.Repositories.Implementations;
using BoardGameBot.Database.Adapter.Repositories.Interfaces;

using Microsoft.Extensions.DependencyInjection;

namespace BoardGameBot.Database.Adapter.Repositories.Extensions
{
	public static class DatabaseExtension
	{
		public static IServiceCollection AddRepositories(this IServiceCollection services)
		{
			services.AddTransient<IGameOwnerRepository, GameOwnerRepository>();
			services.AddTransient<IGameRepository, GameRepository>();
			return services;
		}
	}
}
