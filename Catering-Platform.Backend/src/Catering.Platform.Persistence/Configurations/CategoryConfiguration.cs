using Catering.Platform.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catering.Platform.Persistence.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("category");
        
        builder.HasKey(f => f.Id);
        
        builder.Property(f => f.Name).IsRequired();
        
        builder.Property(f => f.Name).IsRequired(false);
        
        builder.HasMany(d => d.Dishes)
            .WithOne(d => d.Category)
            .HasForeignKey(d => d.CategoryId);
    }
}