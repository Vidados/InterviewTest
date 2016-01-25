using System.Collections.Generic;

namespace InterviewTest.ViewModels
{
    public class NewsletterViewModel
    {
        public List<INewsletterItem> Items { get; set; } = new List<INewsletterItem>();
    }
}