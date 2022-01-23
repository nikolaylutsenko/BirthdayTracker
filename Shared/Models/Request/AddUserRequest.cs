namespace BirthdayTracker.Shared.Models.Request
{
    public record AddUserRequest
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime BirthDay { get; set; }
        public string PositionName { get; set; }
        public string CompanyName { get; set; }
    }
}
