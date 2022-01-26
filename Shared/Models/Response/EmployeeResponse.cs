namespace BirthdayTracker.Shared.Models.Response
{
    public record EmployeeResponse
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string PositionName { get; set; }
        public DateTime BirthDay { get; set; }
    }
}
