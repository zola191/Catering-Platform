namespace Catering.Platform.Domain.Exceptions;

public class SearchByTextException : Exception
{
    public SearchByTextException(string message) : base(message) { }
}
