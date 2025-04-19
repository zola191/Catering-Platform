namespace Catering.Platform.Domain.Exceptions
{
    public class DishNotFoundException : Exception
    {
        public DishNotFoundException() : base("Dish does not exist.") { }

    }
}
