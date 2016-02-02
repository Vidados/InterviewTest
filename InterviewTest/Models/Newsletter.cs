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
        public string Format { get; set; }
        public List<string> ResourceIds { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}