using Microsoft.VisualStudio.TestTools.UnitTesting;
using InterviewTest.Database;
using InterviewTest.Commands;
using InterviewTest.Controllers;
using NSubstitute;
using InterviewTest.Models;
using System.Collections.Generic;
using InterviewTest.Services;
using System.Web.Mvc;

namespace InterviewTest.Tests.Controllers
{
    [TestClass]
    public class NewsletterControllerTests
    {
        private IDatabase _database;
        private ICommandHandler<CreateNewsletterCommand> _createNewsletterCommandHandler;
        private NewsletterController _controller;
        private string _specification;
        private int _count;

        [TestInitialize]
        public void Initialize()
        {
            _specification = "HHHTTHHH";
            _count = 5;

            _database = Substitute.For<IDatabase>();
            _database.GetAll<NewsletterCompositionSpecification>().Returns(
                new List<NewsletterCompositionSpecification>
                {
                    new NewsletterCompositionSpecificationParserService().Parse(_specification)
                });

            _createNewsletterCommandHandler = Substitute.For<ICommandHandler<CreateNewsletterCommand>>();
            _controller = new NewsletterController(_database, _createNewsletterCommandHandler);
        }

        [TestMethod]
        public void ListShouldShowCurrentNewsletterSpecification()
        {
            var result = _controller.List() as ViewResult;
            var model = result.Model as NewsletterListViewModel;

            Assert.AreEqual(_specification, model.Specification);
        }

        [TestMethod]
        public void ListShouldShowDefaultNewsletterSpecificationIfNoneExists()
        {
            _database.GetAll<NewsletterCompositionSpecification>().Returns(
                new List<NewsletterCompositionSpecification>());

            var result = _controller.List() as ViewResult;
            var model = result.Model as NewsletterListViewModel;

            Assert.AreEqual(_specification, model.Specification);
        }

        [TestMethod]
        public void CreateShouldCallHandlerWithCorrectCountAndSpecification()
        {
            _controller.Create(_count, _specification);

            _createNewsletterCommandHandler.Received().Handle(
                Arg.Is<CreateNewsletterCommand>(arg => arg.Count == _count && arg.Specification == _specification));
        }
    }
}
