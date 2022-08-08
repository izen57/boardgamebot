using NLog.Web;

namespace BoardGameBot {
	public static class Program {
		public static void Main(string[] args) {
			var app = CreateHostBuilder(args).Build();
			app.Run();
		}

		private static IHostBuilder CreateHostBuilder(string[] args) {
			var config = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json")
				.Build();

			return Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(webBuilder => {
				webBuilder.UseUrls($"http://0.0.0.0:{config.GetValue<int>("Port")}");
				webBuilder.UseStartup<Startup>();
				webBuilder.UseKestrel();
				webBuilder.UseNLog();
			});
		}
	}
}