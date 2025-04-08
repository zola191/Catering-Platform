using Catering.Platform.Domain.Models;
using Catering.Platform.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Catering.Platform.Persistence;

public class ApplicationDbContext : DbContext
{
    public DbSet<Category> Categories { get; set; }
    public DbSet<Dish> Dishes { get; set; }

    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
        
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DishConfiguration).Assembly);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CategoryConfiguration).Assembly);
    }
}
