namespace BirthdayTracker.Shared;

public record User
{
    public string UserName { get; init; }
    public string Password { get; init; }
}