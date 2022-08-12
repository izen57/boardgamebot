using BoardGameBot.Database.PostgreSQL.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BoardGameBot.Database.PostgreSQL.Configurations
{
	public class GameConfiguration: IEntityTypeConfiguration<Game>
	{
		public void Configure(EntityTypeBuilder<Game> builder)
		{
			builder.HasIndex(game => game.Id)
				.IsUnique();
			builder.HasKey(game => game.Id);
			builder.Property(game => game.Id)
				.IsRequired();
			builder.HasMany(game => game.GameOwners)
				.WithMany(owner => owner.Games);
		}
	}
}