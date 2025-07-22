using System.Collections.ObjectModel;

namespace Catering.Platform.Domain.Exceptions;

public class InvalidPaginationException : Exception
{
    public ReadOnlyCollection<ValidationFailure> Errors { get; }

    public InvalidPaginationException(IEnumerable<ValidationFailure> errors)
        : base("One or more validation failures occurred.")
    {
        Errors = new ReadOnlyCollection<ValidationFailure>(errors.ToList());
    }

    public InvalidPaginationException(ValidationFailure error)
        : this(new[] { error })
    {
    }
}
