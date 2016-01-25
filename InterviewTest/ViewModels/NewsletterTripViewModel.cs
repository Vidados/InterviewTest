namespace InterviewTest.ViewModels
{
    public class NewsletterTripViewModel : INewsletterItem
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Country { get; set; }
        public string HostName { get; set; }
        public string ImageUrl { get; set; }
    }
}