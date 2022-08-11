using BoardGameBot.Database.PostgreSQL.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BoardGameBot.Database.PostgreSQL.Configurations
{
	public class GroupConfiguration: IEntityTypeConfiguration<Group>
	{
		public void Configure(EntityTypeBuilder<Group> builder)
		{
			builder.HasIndex(group => group.Id).IsUnique();
			builder.HasKey(group => group.Id);
			builder.Property(group => group.Id).IsRequired();
			builder.HasMany(group => group.AllMembers).WithOne(gameOwner => gameOwner.GameOwnerGroup);
			builder.HasMany(group => group.AllAdmins).WithOne(gameOwner => gameOwner.GameOwnerGroup);
			builder.HasMany(group => group.AllPolls).WithOne(poll => poll.PollGroup);
		}
	}
}
