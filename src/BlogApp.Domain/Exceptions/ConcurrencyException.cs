namespace BlogApp.Domain.Exceptions;

/// <summary>
/// Optimistic concurrency violation durumunda fırlatılır
/// Bir kayıt başka bir kullanıcı tarafından değiştirildiğinde oluşur
/// </summary>
public sealed class ConcurrencyException : Exception
{
    public ConcurrencyException() 
        : base("The record was modified by another user. Please refresh and try again.")
    {
    }

    public ConcurrencyException(string message) 
        : base(message)
    {
    }

    public ConcurrencyException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }
}
