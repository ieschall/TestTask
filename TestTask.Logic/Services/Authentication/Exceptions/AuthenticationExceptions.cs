namespace TestTask.Logic.Services.Authentication.Exceptions;

public class AuthenticationExceptions : Exception
{
    public AuthenticationExceptions(string message)
        : base(message)
    { }
}