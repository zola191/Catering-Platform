namespace Catering.Platform.Domain.Exceptions;

public class ValidationFailure
{
    public string PropertyName { get; set; }
    public string ErrorMessage { get; set; }

    public ValidationFailure(string propertyName, string errorMessage)
    {
        PropertyName = propertyName;
        ErrorMessage = errorMessage;
    }
}
