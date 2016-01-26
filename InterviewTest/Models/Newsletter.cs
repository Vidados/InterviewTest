using System;
using System.Collections.Generic;
using InterviewTest.Database;

namespace InterviewTest.Models
{
    public class Newsletter : IPersistable
    {
        public Newsletter()
        {
            CreatedAt = DateTime.Now;
        }
        public string Id { get; set; }
        public List<string> TripIds { get; set; }
        public List<string> HostIds { get; set; }
        public string splitsString { get; set; }
        public List<List<string>> LayoutOf { get; set; }
        public DateTime CreatedAt { get; set; }
        public string addclass { get; set; }
    }
}