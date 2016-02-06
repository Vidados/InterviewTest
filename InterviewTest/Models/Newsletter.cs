using System;
using System.Collections.Generic;
using InterviewTest.Database;

namespace InterviewTest.Models
{
    public enum NewsletterItemType
    {
        Trip = 1, Host = 2
    }

    public class NewsletterItem
    {
        public NewsletterItemType Type { get; set; }
        public List<string> Ids { get; set; }
    }

    public class Newsletter : IPersistable
    {
        public Newsletter()
        {
            CreatedAt = DateTime.Now;
        }
        public string Id { get; set; }
        public List<NewsletterItem> Items { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}