using System.Collections.Generic;
using InterviewTest.Database;
using InterviewTest.Models;
using InterviewTest.Services;
using Moq;
using NUnit.Framework;

namespace InterviewTest.Tests.UnitTests
{
    [TestFixture]
    public class NewsletterServiceTest
    {
        private Mock<FileSystemDatabase> _database;
        private NewsletterService _newsletterService;

        [SetUp]
        public void SetUp()
        {
            _database = new Mock<FileSystemDatabase>();
            _newsletterService = new NewsletterService(_database.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _database = null;
            _newsletterService = null;
        }

        private void SetHostMock(string id)
        {
            _database.Setup(x => x.Get<Host>(id)).Returns(new Host {Id=id});
        }
        private void SetTripMock(string id)
        {
            _database.Setup(x => x.Get<Trip>(id)).Returns(new Trip {Id=id});
        }


        [Test]
        public void GetContents_WhenNewsletterWithPattern_ReturnContents()
        {
            // Arrange
            var newsletter = new Newsletter
            {
                Id = "0001",
                DisplayPattern = "HHTTHH",
                HostIds = new List<string> {"0000", "0001", "0002", "0003", "0004" },
                TripIds = new List<string> { "0000","0001","0002", "0003", "0004" }
            };
            
            // Act
            var result = _newsletterService.GetContents(newsletter);
            
            // Assert
            Assert.That(result.Count, Is.EqualTo(newsletter.DisplayPattern.Length));
        }

        [Test]
        public void GetContents_WhenNotExistEnoughHostInList_ReturnRestOfContents()
        {
            // Arrange
            var newsletter = new Newsletter
            {
                Id = "0001",
                DisplayPattern = "HHTTHH",
                HostIds = new List<string> { "0000"},
                TripIds = new List<string> { "0000", "0001", "0002", "0003", "0004" }
            };

            // Act
            var result = _newsletterService.GetContents(newsletter);

            // Assert
            Assert.That(result.Count, Is.EqualTo(3));
        }

        [Test]
        public void GetContents_WhenNotExistEnoughTripInList_ReturnRestOfContents()
        {
            // Arrange
            var newsletter = new Newsletter
            {
                Id = "0001",
                DisplayPattern = "HHTTHH",
                HostIds = new List<string> { "0000", "0001", "0002", "0003", "0004" },
                TripIds = new List<string> { "0000"}
            };

            // Act
            var result = _newsletterService.GetContents(newsletter);

            // Assert
            Assert.That(result.Count, Is.EqualTo(5));
        }

        [Test]
        public void GetContents_WhenPatternIsEmpty_ReturnEmptyValue()
        {
            // Arrange
            var newsletter = new Newsletter
            {
                Id = "0000",
                DisplayPattern = ""
            };

            // Act
            var result = _newsletterService.GetContents(newsletter);

            // Assert
            Assert.That(result, Is.Empty);
        }

        

    }
}
