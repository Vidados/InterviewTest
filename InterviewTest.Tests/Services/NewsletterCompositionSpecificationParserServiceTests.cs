using Microsoft.VisualStudio.TestTools.UnitTesting;
using InterviewTest.Services;
using InterviewTest.Models;
using System.Linq;

namespace InterviewTest.Tests.Services
{
    [TestClass]
    public class NewsletterCompositionSpecificationParserServiceTests
    {
        [TestMethod]
        public void ShouldParseInputTTTHHTTT()
        {
            var service = new NewsletterCompositionSpecificationParserService();

            var expected = new NewsletterCompositionSpecification
            {
                new NewsletterCompositionSpecificationElement
                {
                    Type = NewsletterItemType.Trip,
                    Count = 3
                },
                new NewsletterCompositionSpecificationElement
                {
                    Type = NewsletterItemType.Host,
                    Count = 2
                },
                new NewsletterCompositionSpecificationElement
                {
                    Type = NewsletterItemType.Trip,
                    Count = 3
                }
            };

            var actual = service.Parse("TTTHHTTT");

            Assert.AreEqual(expected.Count, actual.Count);

            foreach (var pair in expected.Zip(actual, (exp, act) => new { Expected = exp, Actual = act }))
            {
                Assert.AreEqual(pair.Expected, pair.Actual);
            }
        }
    }
}
