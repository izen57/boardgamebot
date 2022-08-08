namespace BoardGameBot {
	internal class Startup {
		public Startup(IConfiguration configuration) {
			Configuration = configuration;
		}
		public IConfiguration Configuration { get; }

		public void ConfigureServices(IServiceCollection services) {
			services.AddControllersWithViews();
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
			app.UseRouting();
			app.UseAuthentication();

			app.UseEndpoints(endpoints => {
				endpoints.MapControllerRoute(
					name: "default",
					pattern: "{controller=Home}/{action=Index}/{id?}");
			});

			app.UseExceptionHandler("/Home/Error");
			app.UseHsts();

		}
	}
}