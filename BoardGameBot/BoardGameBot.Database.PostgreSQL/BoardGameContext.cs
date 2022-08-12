using BoardGameBot.Database.PostgreSQL.Configurations;
using BoardGameBot.Database.PostgreSQL.Models;

using Microsoft.EntityFrameworkCore;

namespace BoardGameBot.Database.PostgreSQL
{
	public class BoardGameContext: DbContext
	{
		public DbSet<Game> Games { get; set; }
		public DbSet<GameOwner> GameOwners { get; set; }
		public DbSet<Group> Groups { get; set; }
		public DbSet<Poll> Polls { get; set; }

		public BoardGameContext(DbContextOptions<BoardGameContext> options) : base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.ApplyConfiguration(new GameConfiguration());
			modelBuilder.ApplyConfiguration(new GameOwnerConfiguration());
			modelBuilder.ApplyConfiguration(new GroupConfiguration());
			modelBuilder.ApplyConfiguration(new PollConfiguration());
		}
	}
}
