using System.Linq;
using InterviewTest.Database;
using InterviewTest.Models;
using InterviewTest.Services;
using Microsoft.Ajax.Utilities;
using NUnit.Framework;

namespace InterviewTest.Tests.Services
{
    [TestFixture]
    public class NewsletterServiceTests
    {
        private FileSystemDatabase _db;
        NewsletterService _serv;

        [SetUp]
        public void Setup()
        {
            _db = new FileSystemDatabase();
            _serv = new NewsletterService(_db);
        }

        [Test]
        public void ShouldGetNewsletters()
        {
            var harr = _serv.GetHostsOrderedList(_db).ToList();
            var tarr = _serv.GetTripsOrderedList(_db).ToList();
            var nl = _serv.GetNewsletters(harr, tarr, 'H', 'T', 1).FirstOrDefault();
            var config = _db.Get<ConfigModel>(nl.ConfigId);


            Assert.That(nl, Is.Not.Null);

            foreach (var token in config.ConfigTokens)
            {
                if (token.Equals('H'))
                {
                    Assert.That(nl.HostIds.Contains(harr.First()));
                    harr.Remove(harr.First());
                }
                if (token.Equals('T'))
                {
                    Assert.That(nl.TripIds.Contains(tarr.First()));
                    tarr.Remove(tarr.First());
                }
            }
            
        }
    }
}
