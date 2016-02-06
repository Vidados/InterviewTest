using InterviewTest.Models;

namespace InterviewTest.Services
{
    public interface INewsletterCompositionSpecificationParserService
    {
        NewsletterCompositionSpecification Parse(string input);
    }
}