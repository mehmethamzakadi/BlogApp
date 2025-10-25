namespace BlogApp.Domain.Exceptions;

public class AuthenticationErrorException : Exception
{
    public AuthenticationErrorException() : base("E-Mail veya şifre hatalı!")
    {
    }

    public AuthenticationErrorException(string? message) : base(message)
    {
    }

    public AuthenticationErrorException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}