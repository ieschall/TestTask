namespace TestTask.Api.Controllers.Login.Models;

public record LoginRequest(
    string Username,
    string Password);