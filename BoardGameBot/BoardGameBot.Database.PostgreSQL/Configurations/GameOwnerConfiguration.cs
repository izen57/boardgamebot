using BoardGameBot.Database.PostgreSQL.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BoardGameBot.Database.PostgreSQL.Configurations
{
	public class GameOwnerConfiguration: IEntityTypeConfiguration<GameOwner>
	{
		public void Configure(EntityTypeBuilder<GameOwner> builder)
		{
			builder.HasIndex(owner => owner.Id).IsUnique();
			builder.HasKey(owner => owner.Id);
			builder.Property(owner => owner.Id).IsRequired();
			builder.HasMany(owner => owner.Games).WithMany(game => game.GameOwners);

		}
	}
}
