using Catering.Platform.Domain.Models;
using Catering.Platform.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catering.Platform.Persistence.Configurations
{
    public class TenantConfiguration : IEntityTypeConfiguration<Tenant>
    {
        public void Configure(EntityTypeBuilder<Tenant> builder)
        {
            builder.ToTable("Tenant");

            builder.HasKey(f => f.Id);
            builder.HasIndex(f => f.Name);

            builder.Property(f => f.Name).IsRequired().HasMaxLength(Constants.MAX_TEXT_LENGTH_200);
            builder.Property(f => f.BlockReason).IsRequired(false).HasMaxLength(Constants.MAX_TEXT_LENGTH_500);
        }
    }
}
