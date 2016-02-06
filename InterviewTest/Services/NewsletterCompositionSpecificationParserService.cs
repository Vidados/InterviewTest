using InterviewTest.Models;
using System;

namespace InterviewTest.Services
{
    public class NewsletterCompositionSpecificationParserService : INewsletterCompositionSpecificationParserService
    {
        public NewsletterCompositionSpecification Parse(string input)
        {
            var composition = new NewsletterCompositionSpecification();
            NewsletterItemType? previousType = null;
            NewsletterCompositionSpecificationElement element = null;

            foreach (var c in input)
            {
                var currentType = ParseElementType(c);

                if (!previousType.HasValue || previousType != currentType)
                {
                    element = new NewsletterCompositionSpecificationElement
                    {
                        Type = currentType,
                        Count = 1
                    };
                    composition.Add(element);
                }
                else
                {
                    element.Count += 1;
                }

                previousType = currentType;
            }

            return composition;
        }

        private static NewsletterItemType ParseElementType(char input)
        {
            switch (input)
            {
                case 'T':
                case 't':
                    return NewsletterItemType.Trip;
                case 'H':
                case 'h':
                    return NewsletterItemType.Host;
                default:
                    throw new ArgumentOutOfRangeException("input", $"Expected 'T', 't', 'H', or 'h'.  Received '{input}'");
            }
        }
    }
}