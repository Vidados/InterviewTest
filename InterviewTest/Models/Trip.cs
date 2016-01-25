using InterviewTest.Database;

namespace InterviewTest.Models
{
    public class Trip : IPersistable, IContent
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public string HostId { get; set; }
        public string ImageUrl { get; set; }
    }
}