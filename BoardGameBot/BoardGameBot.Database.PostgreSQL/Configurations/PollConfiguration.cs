using BoardGameBot.Database.PostgreSQL.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BoardGameBot.Database.PostgreSQL.Configurations
{
	public class PollConfiguration: IEntityTypeConfiguration<Poll>
	{
		public void Configure(EntityTypeBuilder<Poll> builder)
		{
			builder.HasIndex(poll => poll.Id).IsUnique();
			builder.HasKey(poll => poll.Id);
			builder.Property(poll => poll.Id).IsRequired();
			builder.HasOne(poll => poll.PollGroup).WithMany(group => group.AllPolls);
		}
	}
}
