namespace TestTask.Logic.Services.Authentication.Responses;

public record AuthenticationResult(
    string AccessToken,
    string RefreshToken);