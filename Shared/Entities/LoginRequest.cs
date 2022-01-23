namespace BirthdayTracker.Shared.Entities;

public record LoginRequest
{
    public string UserName { get; init; }
    public string Password { get; init; }
}