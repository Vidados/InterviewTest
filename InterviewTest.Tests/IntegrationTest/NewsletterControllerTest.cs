using System.Web.Mvc;
using InterviewTest.Controllers;
using InterviewTest.Database;
using InterviewTest.Services;
using NUnit.Framework;

namespace InterviewTest.Tests.IntegrationTest
{
    [TestFixture]
    public class NewsletterControllerTest
    {
        private FileSystemDatabase _database;
        private HostService _hostService;
        private TripService _tripService;
        private NewsletterService _newsletterService;
        private NewsletterController _newsletterController;

        [SetUp]
        public void SetUp()
        {
            _database = new FileSystemDatabase();
            _hostService = new HostService(_database);
            _tripService = new TripService(_database);
            _newsletterService = new NewsletterService(_database);
            _newsletterController = new NewsletterController(_newsletterService, 
                                                             _hostService, 
                                                             _tripService);
        }

        [TearDown]
        public void TearDown()
        {
            _database = null;
            _hostService = null;
            _tripService = null;
            _newsletterService = null;
            _newsletterController = null;
        }

        [Test]
        public void Create_WhenCall_CreateDataWithoutErrors()
        {
            // Act
            var result = _newsletterController.Create(10, "HHTTHH") as RedirectToRouteResult;
            
            // Assert
            Assert.IsTrue(_newsletterController.TempData.ContainsKey("notification"));
            Assert.NotNull(result, "Not a redirect result");
            Assert.IsFalse(result.Permanent);
            Assert.AreEqual("list", result.RouteValues["Action"]);
        }

        [Test]
        public void Sample_WhenCall_ReturnNewsletterViewWithSampleData()
        {
            // Act
            var result = _newsletterController.Sample() as ViewResult;

            // Assert
            Assert.IsNotNull(result, "Not a viewResult");
            Assert.IsNotNull(result.Model, "Empty a model");
            Assert.AreEqual("Newsletter", result.ViewName);
        }

        [Test]
        public void Display_WhenDataExists_ReturnViewResultWithData()
        {
            // Arrange
            _newsletterController.Create(10, "HHTTHH"); // sample data
            
            // Act
            var result = _newsletterController.Display("00000") as ViewResult;

            // Assert
            Assert.IsNotNull(result, "Not a viewResult");
            Assert.IsNotNull(result.Model, "Empty a model");
            Assert.AreEqual("Newsletter", result.ViewName);
        }

        [Test]
        public void Display_WhenDataNotExist_RedirectToList()
        {
            // Arrange
            _newsletterController.DeleteAll(); // delete all data

            // Act
            var result = _newsletterController.Display("00000") as RedirectToRouteResult;

            // Assert
            Assert.NotNull(result, "Not a redirect result");
            Assert.IsFalse(result.Permanent);
            Assert.AreEqual("list", result.RouteValues["Action"]);
        }
        

        [Test]
        public void List_WhenCall_ReturnListViewWithData()
        {
            // Act
            ViewResult result = _newsletterController.List() as ViewResult;

            // Assert
            Assert.IsNotNull(result, "Not a viewResult");
            Assert.IsNotNull(result.Model, "Empty a model");
        }

        [Test]
        public void DeleteAll_WhenCall_ReturnRedirectViewAfterDeleteData()
        {
            // Act
            RedirectToRouteResult result = _newsletterController.DeleteAll() as RedirectToRouteResult;

            // Assert
            Assert.IsTrue(_newsletterController.TempData.ContainsKey("notification"));
            Assert.NotNull(result, "Not a redirect result");
            Assert.IsFalse(result.Permanent);
            Assert.AreEqual("list", result.RouteValues["Action"]);
        }
    }
}
