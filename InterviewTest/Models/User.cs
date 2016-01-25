using InterviewTest.Database;

namespace InterviewTest.Controllers
{
    public class User : IPersistable
    {
        public string Name { get; set; }
        public string Id { get; set; }
    }
}