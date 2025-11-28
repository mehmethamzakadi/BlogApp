namespace BlogApp.Domain.Exceptions;

/// <summary>
/// Domain validation hatalarını temsil eden exception.
/// Application katmanında FluentValidation sonuçlarından oluşturulabilir.
/// </summary>
public class ValidationException : ApplicationException
{
    public List<string> Errors { get; }

    public ValidationException(string message) : base(message)
    {
        Errors = new List<string> { message };
    }

    public ValidationException(IEnumerable<string> errors) : base(errors.FirstOrDefault() ?? "Validation failed")
    {
        Errors = errors.ToList();
    }

    public ValidationException(string message, IEnumerable<string> errors) : base(message)
    {
        Errors = errors.ToList();
    }
}
