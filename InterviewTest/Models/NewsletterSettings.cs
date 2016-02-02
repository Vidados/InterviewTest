using System;
using InterviewTest.Database;

namespace InterviewTest.Models
{
    public class NewsletterSettings : IPersistable
    {
        public string Id { get; set; }
        public string NewsletterFormat { get; set; }
    }
}