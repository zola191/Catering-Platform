namespace Catering.Platform.Domain.Exceptions
{
    public class CategoryNotFoundException : Exception
    {
        public CategoryNotFoundException() : base("Category does not exist.") { }

    }
}
