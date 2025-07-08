using Catering.Platform.Domain.Models;
using Catering.Platform.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catering.Platform.Persistence.Configurations;

public class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.ToTable("Company");

        builder.HasKey(f => f.Id);

        builder.Property(f => f.Name)
            .IsRequired()
            .HasMaxLength(Constants.MAX_TEXT_LENGTH_200);

        builder.Property(f => f.TaxNumber)
            .IsRequired()
            .HasMaxLength(Constants.MAX_TEXT_LENGTH_20);

        builder.Property(f => f.Phone)
            .IsRequired(false)
            .HasMaxLength(Constants.MAX_TEXT_LENGTH_20);

        builder.Property(f => f.Email)
            .IsRequired(false)
            .HasMaxLength(Constants.MAX_TEXT_LENGTH_100);

        builder.Property(f=>f.IsBlocked)
            .HasDefaultValue(false);

        builder.HasIndex(f => f.TaxNumber).IsUnique();
        builder.HasIndex(f => f.TenantId);

        builder.HasOne(e => e.Tenant)
              .WithMany(t => t.Companies)
              .HasForeignKey(e => e.TenantId)
              .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Address)
              .WithMany(a => a.Companies)
              .HasForeignKey(e => e.AddressId)
              .OnDelete(DeleteBehavior.Restrict);
    }
}
