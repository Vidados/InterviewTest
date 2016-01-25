using System.Web.Mvc;
using InterviewTest.Controllers;
using NUnit.Framework;

namespace InterviewTest.Tests.IntegrationTest
{
    [TestFixture]
    public class HomeControllerTest
    {


        [Test]
        public void Index()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }
    }
}
