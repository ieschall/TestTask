namespace TestTask.DataAccess.Models;

public record UserRefreshData(
    string Username,
    DateTimeOffset ValidUntil);