using Catering.Platform.Domain.Models;
using Catering.Platform.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catering.Platform.Persistence.Configurations;

public class DishConfiguration: IEntityTypeConfiguration<Dish>
{
    public void Configure(EntityTypeBuilder<Dish> builder)
    {
        builder.ToTable("Dish");
        
        builder.HasKey(f => f.Id);
        
        builder.Property(f => f.Name)
            .IsRequired()
            .HasMaxLength(Constants.MAX_TEXT_LENGTH_256);
        
        builder.Property(f => f.Description)
            .IsRequired()
            .HasMaxLength(Constants.MAX_TEXT_LENGTH_2000);
        
        builder.Property(f => f.Price).IsRequired();
        
        builder.Property(f => f.ImageUrl).IsRequired(false);
        
        builder.Property(f => f.IsAvailable).IsRequired();
        
        builder.OwnsOne(f => f.Ingredients, ingredients =>
        {
            ingredients.ToJson();
        });

        builder.OwnsOne(f => f.Allergens, allergens =>
        {
            allergens.ToJson();
        });

        builder.HasOne(d => d.Category)
            .WithMany(c => c.Dishes)
            .HasForeignKey(d => d.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}