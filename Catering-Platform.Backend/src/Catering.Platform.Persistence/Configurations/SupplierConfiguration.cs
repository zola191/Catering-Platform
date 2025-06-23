using Catering.Platform.Domain.Models;
using Catering.Platform.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catering.Platform.Persistence.Configurations;

public class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> builder)
    {
        builder.Property(f => f.CompanyId).IsRequired();
        builder.Property(f => f.Position).HasMaxLength(Constants.MAX_TEXT_LENGTH_64);

        builder.Property(f => f.CompanyId).HasColumnName("Supplier_CompanyId");
    }
}
