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
			builder.HasMany(group => group.Members)
				.WithOne(gameOwner => gameOwner.GroupMember)
				.HasForeignKey(gameOwner => gameOwner.GroupMemberId);
			builder.HasMany(group => group.Admins)
				.WithOne(gameOwner => gameOwner.GroupAdmin)
				.HasForeignKey(gameOwner => gameOwner.GroupAdminId);
			builder.HasMany(group => group.Polls)
				.WithOne(poll => poll.Group)
				.HasForeignKey(poll => poll.GroupId);
		}
	}
}
