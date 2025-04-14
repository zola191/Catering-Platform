namespace Catering.Platform.Domain.Exceptions;

public class CategoryAlreadyExistException : Exception
{
    public CategoryAlreadyExistException() : base("Category already exist.")
    {
    }
}