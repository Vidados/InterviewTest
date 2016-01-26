using InterviewTest.Database;

namespace InterviewTest.Models
{
    public class Trip : IPersistable
    {
        public Trip()
        {
            used = 0;
        }
        public string Id { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public string HostId { get; set; }
        public string ImageUrl { get; set; }
        public string addclass { get; set; }
        public int used { get; set; }
    }

    public class Host : IPersistable
    {
      public Host(){
            used = 0;
            }
        public string Id { get; set; }
        public string Name { get; set; }
        public string Job { get; set; }
        public string ImageUrl { get; set; }
        public string addclass { get; set; }
        public int used { get; set; }
    }
}