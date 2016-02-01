using InterviewTest.Database;
using System;
using System.Collections.Generic;

namespace InterviewTest.Models
{
    public class Settings : IPersistable
    {
        public Settings()
        {
            CreatedAt = DateTime.Now;
        }

        public string Id { get; set; }
        public IList<NewsletterItem> Layout { get; set; } = new List<NewsletterItem>();
        public DateTime CreatedAt { get; set; }
    }
}