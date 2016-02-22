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
            var harr = _serv.GetHostsOrderedDict(_db);
            var tarr = _serv.GetTripsOrderedDict(_db);
            var nl = _serv.GetNewsletters(harr, tarr, 'H', 'T', 1).FirstOrDefault();
            var config = _db.Get<ConfigModel>(nl.ConfigId);


            Assert.That(nl, Is.Not.Null);

            foreach (var token in config.ConfigTokens)
            {
                if (token.Equals('H'))
                {
                    Assert.That(nl.HostIds.Contains(harr.Keys.First()));
                    harr.Remove(harr.Keys.First());
                }
                if (token.Equals('T'))
                {
                    Assert.That(nl.TripIds.Contains(tarr.Keys.First()));
                    tarr.Remove(tarr.Keys.First());
                }
            }
            
        }
    }
}
