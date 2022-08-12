using BoardGameBot.Core;
using BoardGameBot.Database.Adapter.Converts;
using BoardGameBot.Database.Adapter.Extensions;
using BoardGameBot.Database.PostgreSQL;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using TelegramBotService.Extensions;

namespace BoardGameBot
{
	internal class Startup
	{
		public IConfiguration Configuration { get; }
		public IServiceScopeFactory _serviceScopeFactory { get; }
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public void ConfigureServices(IServiceCollection services)
		{
			services.AddControllersWithViews();

			var connectionString = Configuration.GetConnectionString("BoardGameConnection");
			services.AddSingleton<IContextFactory>(new ContextFactory(connectionString));
			services.AddDbContext<BoardGameContext>(options => options.UseNpgsql(connectionString));

			services.AddAutoMapper(c => 
				c.AddProfile<GameBoardAutoMapperProfile>(), typeof(Startup)
			);
			services.AddRepositories();
			services.AddTelegramBotService(Configuration);
			services.AddHostedService<CoreService>();
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			app.UseRouting();
			UpdateDatabase(app);
			app.UseAuthentication();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllerRoute(
					name: "default",
					pattern: "{controller=Home}/{action=Index}/{id?}");
			});

			app.UseExceptionHandler("/Home/Error");
			app.UseHsts();
		}

		private static void UpdateDatabase(IApplicationBuilder app)
		{
			using var serviceScope = app.ApplicationServices
				.GetRequiredService<IServiceScopeFactory>()
				.CreateScope();
			using var context = serviceScope.ServiceProvider
				.GetService<BoardGameContext>();
			context?.Database.Migrate();
		}
	}
}