using InterviewTest.Database;

namespace InterviewTest.Models
{
    public class User : IPersistable
    {
        public string Name { get; set; }
        public string Id { get; set; }
    }
}