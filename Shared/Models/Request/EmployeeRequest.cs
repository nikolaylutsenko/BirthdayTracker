namespace BirthdayTracker.Shared.Models.Request
{
    public record EmployeeRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Position { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}
