namespace BirthdayTracker.Shared;

public record User
{
    public string UserName { get; init; }
    public string Password { get; init; }
}

public record AppUser
{
    public string UserName { get; init; }
    public string Password { get; init; }
    public int CompanyId { get; init; }

    // navigation properties
    public Company Company { get; init; }

}