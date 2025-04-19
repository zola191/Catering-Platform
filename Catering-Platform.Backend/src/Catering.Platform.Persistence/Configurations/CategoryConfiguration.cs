using Catering.Platform.Domain.Models;
using Catering.Platform.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catering.Platform.Persistence.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("category");
        
        builder.HasKey(f => f.Id);
        
        builder.Property(f => f.Name).IsRequired().HasMaxLength(Constants.MAX_LOW_TEXT_LENGTH);
        
        builder.Property(f => f.Description).IsRequired(false).HasMaxLength(-1); // разворачивается nvarchar(max) в бд.
                                                                                 // (посмотреть по varchar и nvarchar)
        
        builder.HasMany(d => d.Dishes)
            .WithOne(d => d.Category)
            .HasForeignKey(d => d.CategoryId);
    }
}