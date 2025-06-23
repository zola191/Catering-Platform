using Catering.Platform.Domain.Models;
using Catering.Platform.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catering.Platform.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(f => f.Id);

        builder.HasOne(u => u.Tenant)
               .WithMany(t => t.Users)
               .HasForeignKey(u => u.TenantId);

        builder.Property(f => f.FirstName)
            .IsRequired()
            .HasMaxLength(Constants.MAX_TEXT_LENGTH_128);

        builder.Property(f => f.LastName)
            .IsRequired()
            .HasMaxLength(Constants.MAX_TEXT_LENGTH_128);

        builder.Property(f => f.MiddleName)
            .HasMaxLength(Constants.MAX_TEXT_LENGTH_128);

        builder.Property(f => f.Email)
            .IsRequired()
            .HasMaxLength(Constants.MAX_TEXT_LENGTH_100);

        builder.Property(f => f.Phone)
               .IsRequired()
               .HasMaxLength(Constants.MAX_TEXT_LENGTH_20);

        builder.Property(f => f.PasswordHash)
               .IsRequired();

        builder.Property(f => f.IsBlocked)
               .IsRequired()
               .HasDefaultValue(false);

        builder.Property(f => f.BlockReason)
               .HasMaxLength(Constants.MAX_TEXT_LENGTH_512);

        builder.Property(f => f.CreatedAt)
               .IsRequired();

        builder.Property(f => f.UpdatedAt)
               .ValueGeneratedOnUpdate();

        builder.HasDiscriminator<string>("UserType")
               .HasValue<Supplier>("Supplier")
               .HasValue<Customer>("Customer")
               .HasValue<Broker>("Broker");

        builder.HasIndex(u => u.Email)
               .IsUnique();
    }
}
