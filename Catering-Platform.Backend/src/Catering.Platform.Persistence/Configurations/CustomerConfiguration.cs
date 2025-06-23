using Catering.Platform.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catering.Platform.Persistence.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.HasOne(c => c.Address)
               .WithOne(a => a.Customer)
               .HasForeignKey<Customer>(c => c.AddressId);

        builder.Property(f => f.CompanyId).HasColumnName("Customer_CompanyId");

        builder.Property(f => f.TaxNumber).IsRequired();
    }
}
