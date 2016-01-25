using InterviewTest.Database;

namespace InterviewTest.Models
{
    public class Host : IPersistable
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Job { get; set; }
        public string ImageUrl { get; set; }
    }
}