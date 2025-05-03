using Catering.Platform.Domain.Models;
using Catering.Platform.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catering.Platform.Persistence.Configurations
{
    public class AddressConfiguration : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.ToTable("Address");
            builder.Property(f => f.Id).IsRequired();
            builder.HasKey(f => f.Id);
            builder.Property(f => f.Country).IsRequired().HasMaxLength(Constants.MAX_TEXT_LENGTH_64);
            builder.Property(f => f.StreetAndBuilding).IsRequired().HasMaxLength(Constants.MAX_TEXT_LENGTH_256);
            builder.Property(f => f.Zip).IsRequired().HasMaxLength(Constants.MAX_TEXT_LENGTH_6);
            builder.Property(f => f.City).IsRequired().HasMaxLength(Constants.MAX_TEXT_LENGTH_64);
            builder.Property(f => f.Region).HasMaxLength(Constants.MAX_TEXT_LENGTH_64);
            builder.Property(f => f.Comment).HasMaxLength(Constants.MAX_TEXT_LENGTH_256);
            builder.Property(f => f.Description).HasMaxLength(Constants.MAX_TEXT_LENGTH_512);
            builder.Property(f => f.CreatedAt).IsRequired();
            builder.Property(f => f.UpdatedAt);


            builder.HasOne(f => f.Tenant).WithMany(f => f.Addresses).HasForeignKey(f => f.TenantId);

            builder.HasIndex(f => f.TenantId);
        }
    }
}
