namespace Catering.Platform.Domain.Models
{
    public class Category : Entity
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public ICollection<Dish> Dishes { get; set; } = new List<Dish>();
    }
}
