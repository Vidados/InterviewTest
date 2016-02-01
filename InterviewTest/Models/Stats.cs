using InterviewTest.Database;
using System.Collections.Generic;

namespace InterviewTest.Models
{
    public class Stats : IPersistable
    {
        public string Id { get; set; }
        public IDictionary<string, int> Trips { get; set; } = new Dictionary<string, int>();
        public IDictionary<string, int> Hosts { get; set; } = new Dictionary<string, int>();
    }
}