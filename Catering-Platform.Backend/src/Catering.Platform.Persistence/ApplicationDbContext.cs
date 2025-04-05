using Catering.Platform.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Catering.Platform.Persistence;

public class ApplicationDbContext : DbContext
{
    public DbSet<Category> Categories { get; set; }

    public ApplicationDbContext(DbContextOptions options) : base(options)
    {

    }
}
